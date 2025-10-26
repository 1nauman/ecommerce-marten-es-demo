using Domain.SharedKernel;

namespace Domain.ShoppingCart;

// Event definition for when a new shopping cart is opened
public record ShoppingCartOpened(
    Guid ShoppingCartId,
    Guid CustomerId
) : DomainEvent;

// Event definition for when a product item is added to the cart
public record ProductItemAddedToShoppingCart(
    Guid ShoppingCartId,
    Guid ProductId,
    int Quantity,
    Money Price
) : DomainEvent;

// Event definition for when a product item is removed from the cart
public record ProductItemRemovedFromShoppingCart(
    Guid ShoppingCartId,
    Guid ProductId
) : DomainEvent;

// Event definition for when the shopping cart is confirmed (checked out)
public record ShoppingCartConfirmed(
    Guid ShoppingCartId,
    DateTime ConfirmedAt
) : DomainEvent;