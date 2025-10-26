using Domain.SharedKernel;

namespace Domain.Product;

public class Product : AggregateRoot
{
    private Product()
    {
    }

    public static Product Create(string name, Money price)
    {
        var product = new Product();
        var @event = new ProductCreated(Guid.NewGuid(), name, price);
        product.Raise(@event);
        return product;
    }

    public string Name { get; private set; } = string.Empty;

    public Money Price { get; private set; } = Money.Invalid;

    protected override void Dispatch(DomainEvent @event) => Apply((dynamic)@event);

    private void Apply(ProductCreated @event)
    {
        Id = @event.ProductId;
        Name = @event.Name;
        Price = @event.Price;
    }
}