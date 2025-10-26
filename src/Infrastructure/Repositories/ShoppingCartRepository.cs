using Application.Abstractions;
using Domain.ShoppingCart;
using Marten;

namespace Infrastructure.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly IDocumentSession _session;

    public ShoppingCartRepository(IDocumentSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }

    public async Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Marten's event sourcing support is first-class.
        // This will fetch all the events for the given stream ID
        // and replay them to construct the current state of the ShoppingCart aggregate.
        return await _session.Events.AggregateStreamAsync<ShoppingCart>(id, token: cancellationToken);
    }

    public Task AddAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
    {
        // Get the uncommitted events from our aggregate
        var events = shoppingCart.GetUncommittedEvents();

        // Append the events to a new stream. Marten will automatically
        // use the shoppingCart.Id as the stream identifier.
        _session.Events.StartStream<ShoppingCart>(shoppingCart.Id, events);

        // Save the changes to the database
        return _session.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
    {
        var events = shoppingCart.GetUncommittedEvents().ToArray<object>();

        // Use AppendOptimistic. It tells Marten to perform an optimistic concurrency
        // check using the version of the stream that was originally loaded into the session.
        // No manual version calculation is needed.
        await _session.Events.AppendOptimistic(shoppingCart.Id, cancellationToken, events);

        await _session.SaveChangesAsync(cancellationToken);
    }
}