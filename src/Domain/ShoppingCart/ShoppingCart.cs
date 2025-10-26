using Domain.SharedKernel;

namespace Domain.ShoppingCart;

public class ShoppingCart : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public ShoppingCartStatus Status { get; private set; }

    private readonly List<ShoppingCartItem> _items = new();
    public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();

    /// <summary>
    /// A private constructor is essential for event sourcing to ensure
    /// that the aggregate can only be created by replaying events.
    /// </summary>
    private ShoppingCart() { }

    // --- Event Dispatching ---

    /// <summary>
    /// This is the required implementation of the dispatch "bridge".
    /// It uses dynamic dispatch to invoke the correct private Apply method
    /// based on the runtime type of the event.
    /// </summary>
    /// <param name="event">The domain event to apply.</param>
    protected override void Dispatch(DomainEvent @event) => Apply((dynamic)@event);

    // --- Public Command Methods ---
    // These methods represent the business operations that can be performed on the aggregate.
    // They contain business logic, enforce invariants, and raise events upon success.

    public static ShoppingCart Open(Guid customerId)
    {
        var cart = new ShoppingCart();
        var @event = new ShoppingCartOpened(Guid.NewGuid(), customerId);
        cart.Raise(@event);
        return cart;
    }

    public void AddProduct(Guid productId, int quantity, Money price)
    {
        if (Status != ShoppingCartStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending shopping cart.");

        var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem != null)
        {
            if (existingItem.Price.Currency != price.Currency)
                throw new InvalidOperationException("Cannot add item with a different currency.");
            existingItem.AddQuantity(quantity);
        }
        else
        {
            var @event = new ProductItemAddedToShoppingCart(Id, productId, quantity, price);
            Raise(@event);
        }
    }

    // --- Private State Mutators ---
    // These methods are the handlers that mutate the aggregate's state in response to an event.
    // They should contain no business logic and only set property values.

    // A TOUGH LESSON LEARNT:
    // For Marten's default live aggregation to work correctly, these state-mutating
    // methods MUST be named "Apply". Marten's convention-based discovery looks for
    // methods with this specific name.

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