using Domain.SharedKernel;

namespace Domain.Order;

public class Order : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public IReadOnlyList<OrderLine> Lines { get; private set; } = new List<OrderLine>();
    public Money TotalPrice { get; private set; } = Money.Create(0, "USD");

    private Order() { }

    protected override void Dispatch(DomainEvent @event) => Apply((dynamic)@event);

    public static Order Place(
        Guid customerId,
        IReadOnlyList<OrderLine> orderLines,
        Money totalPrice)
    {
        var order = new Order();
        var @event = new OrderPlaced(
            Guid.NewGuid(),
            customerId,
            orderLines,
            totalPrice,
            DateTime.UtcNow);

        order.Raise(@event);
        return order;
    }

    private void Apply(OrderPlaced @event)
    {
        Id = @event.OrderId;
        CustomerId = @event.CustomerId;
        Lines = @event.OrderLines;
        TotalPrice = @event.TotalPrice;
    }
}