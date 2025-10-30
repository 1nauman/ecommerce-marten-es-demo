using Application.Order.Commands;
using Application.ReadModels;

using Domain.Order;
using Domain.SharedKernel;
using Domain.ShoppingCart;

using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;

using Marten;
using Marten.Subscriptions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Infrastructure.Processes;

public class ShoppingCartCheckoutProcessManager : ISubscription
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShoppingCartCheckoutProcessManager> _logger;

    public ShoppingCartCheckoutProcessManager(IMediator mediator, ILogger<ShoppingCartCheckoutProcessManager> logger)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(logger);

        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IChangeListener> ProcessEventsAsync(
        EventRange page,
        ISubscriptionController controller,
        IDocumentOperations operations,
        CancellationToken cancellationToken)
    {
        // Find all the ShoppingCartConfirmed events in this batch
        var confirmedEvents = page.Events.Where(e => e.Data is ShoppingCartConfirmed);

        foreach (var @event in confirmedEvents)
        {
            if (@event.Data is ShoppingCartConfirmed confirmed)
            {
                await HandleShoppingCartConfirmedAsync(confirmed, operations, cancellationToken);
            }
        }

        return NullChangeListener.Instance;
    }

    private async Task HandleShoppingCartConfirmedAsync(
        ShoppingCartConfirmed confirmed,
        IDocumentOperations operations,
        CancellationToken cancellationToken)
    {
        // --- Idempotency check ---
        var existingProcess =
            await operations.LoadAsync<ShoppingCartCheckoutProcess>(confirmed.ShoppingCartId, cancellationToken);

        if (existingProcess is not null && existingProcess.Status != ProcessStatus.Failed)
        {
            _logger.LogWarning("Shopping cart checkout process already exists for ShoppingCartId: {ShoppingCartId}",
                confirmed.ShoppingCartId);

            return; // Already handled, so skip further processing
        }

        // Create the process for the ShoppingCartConfirmed event
        var process = existingProcess ?? new ShoppingCartCheckoutProcess { CartId = confirmed.ShoppingCartId };
        var orderId = Guid.NewGuid();
        process.OrderId = orderId;

        try
        {
            _logger.LogInformation("Starting cart checkout process for Cart {CartId}, creating Order {OrderId}",
                confirmed.ShoppingCartId, orderId);
            process.Status = ProcessStatus.Started;
            operations.Store(process); // Store the initial state

            var cartSummary =
                await operations.LoadAsync<ShoppingCartSummary>(confirmed.ShoppingCartId, cancellationToken);
            if (cartSummary is null)
            {
                throw new InvalidOperationException(
                    $"Could not find ShoppingCartSummary for CartId {confirmed.ShoppingCartId}. The read model may be lagging.");
            }

            var orderLines = cartSummary.Items
                .Select(item =>
                    new OrderLine(item.ProductId, item.Quantity, new Money(item.Price.Amount, item.Price.Currency)))
                .ToArray();

            var command = new PlaceOrderCommand(cartSummary.CustomerId, orderLines,
                new Money(cartSummary.TotalPrice.Amount, cartSummary.TotalPrice.Currency));

            await _mediator.Send(command, cancellationToken);

            process.Status = ProcessStatus.OrderPlaced;
            _logger.LogInformation("Successfully placed Order {OrderId} for Cart {CartId}", orderId,
                confirmed.ShoppingCartId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to process checkout for Cart {CartId}", confirmed.ShoppingCartId);
            process.Status = ProcessStatus.Failed;
            process.ErrorMessage = e.Message;
            // Re-throw to let Marten's error handling and retry logic take over.
            throw;
        }
        finally
        {
            operations.Store(process);
        }
    }
}