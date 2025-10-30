using Domain.SharedKernel;

namespace Domain.ShoppingCart;

public class ShoppingCartItem
{
    public Guid ProductId { get; }

    public int Quantity { get; private set; }

    public Money Price { get; }

    // private constructor
    private ShoppingCartItem(Guid productId, int quantity, Money price)
    {
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    public static ShoppingCartItem Create(Guid productId, int quantity, Money price)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        return new ShoppingCartItem(productId, quantity, price);
    }

    internal void AddQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        Quantity += quantity;
    }

    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(newQuantity), "New quantity must be greater than zero.");

        Quantity = newQuantity;
    }
}