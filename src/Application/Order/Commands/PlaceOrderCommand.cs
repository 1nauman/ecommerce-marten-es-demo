using Application.Abstractions;

using DO = Domain.Order;

using Domain.SharedKernel;

using MediatR;

namespace Application.Order.Commands;

public record PlaceOrderCommand(
    Guid CustomerId,
    DO.OrderLine[] OrderLines,
    Money TotalPrice
) : IRequest<Guid>;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;

    public PlaceOrderCommandHandler(IOrderRepository orderRepository)
    {
        ArgumentNullException.ThrowIfNull(orderRepository);
        _orderRepository = orderRepository;
    }

    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var order = DO.Order.Place(
            request.CustomerId,
            request.OrderLines,
            request.TotalPrice);

        await _orderRepository.AddAsync(order, cancellationToken);

        return order.Id;
    }
}