using Domain.SharedKernel;

namespace Domain.ShoppingCart;

public class ShoppingCart : AggregateRoot
{
    private readonly List<ShoppingCartItem> _items = [];

    private ShoppingCart()
    {
    }

    public Guid CustomerId { get; private set; }

    public ShoppingCartStatus Status { get; private set; }

    public IReadOnlyCollection<ShoppingCartItem> Items => _items.AsReadOnly();

    public static ShoppingCart Open(Guid customerId)
    {
        var cart = new ShoppingCart();
        var @event = new ShoppingCartOpened(cart.Id, customerId);
        cart.Raise(@event);
        return cart;
    }

    public void AddProduct(Guid productId, int quantity, Money price)
    {
        if (Status != ShoppingCartStatus.Pending)
        {
            throw new InvalidOperationException("Cannot add product to a cart that is not in pending state.");
        }

        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            // For simplicity, we'll just update quantity. A real system might have more complex logic.
            // This logic does not generate a new event in this simplified example, but it could.
            existingItem.AddQuantity(quantity);
        }
        else
        {
            var @event = new ProductItemAddedToShoppingCart(Id, productId, quantity, price);
            Raise(@event);
        }
    }

    // --- Private State Mutators (Apply methods) ---

    private void Apply(ShoppingCartOpened @event)
    {
        Id = @event.ShoppingCartId;
        CustomerId = @event.CustomerId;
        Status = ShoppingCartStatus.Pending;
    }

    private void Apply(ProductItemAddedToShoppingCart @event)
    {
        var newItem = ShoppingCartItem.Create(@event.ProductId, @event.Quantity, @event.Price);
        _items.Add(newItem);
    }
}

public enum ShoppingCartStatus
{
    Pending,
    Confirmed,
    Cancelled
}