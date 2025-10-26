using Application.ReadModels;
using Domain.ShoppingCart;
using Marten.Events.Aggregation;

namespace Infrastructure.Projections;

public class ShoppingCartSummaryProjection : SingleStreamProjection<ShoppingCartSummary, Guid>
{
    // The Apply for ShoppingCartOpened now initializes the MoneyModel
    public ShoppingCartSummary Apply(ShoppingCartOpened @event, ShoppingCartSummary current)
    {
        return current with
        {
            Id = @event.ShoppingCartId,
            CustomerId = @event.CustomerId,
            Status = ShoppingCartStatus.Pending.ToString(),
            // Initialize with zero USD, assuming USD is the default.
            // A more advanced system might get the currency from the customer's profile.
            TotalPrice = MoneyModel.Invalid
        };
    }

    public ShoppingCartSummary Apply(ProductItemAddedToShoppingCart @event, ShoppingCartSummary current)
    {
        var existingItem = current.Items.FirstOrDefault(i => i.ProductId == @event.ProductId);
        var updatedItems = new List<ShoppingCartSummary.ShoppingCartItemSummary>(current.Items);

        if (existingItem != null)
        {
            updatedItems.Remove(existingItem);
            updatedItems.Add(existingItem with { Quantity = existingItem.Quantity + @event.Quantity });
        }
        else
        {
            updatedItems.Add(new ShoppingCartSummary.ShoppingCartItemSummary
            {
                ProductId = @event.ProductId,
                Quantity = @event.Quantity,
                // Map from the domain Value Object to the read model DTO
                Price = MoneyModel.FromDomain(@event.Price)
            });
        }

        // Recalculate the total price
        var totalPriceAmount = updatedItems.Sum(item => item.Price.Amount * item.Quantity);

        // Assume all items have the same currency for simplicity
        // Ideally, we would get the currency from the customer's profile or probably do a currency conversion based on the shopping cart's currency
        var currency = updatedItems.FirstOrDefault()?.Price.Currency ?? "USD";

        return current with
        {
            Items = updatedItems,
            TotalPrice = new MoneyModel(totalPriceAmount, currency)
        };
    }

    public ShoppingCartSummary Apply(ProductItemQuantityUpdated @event, ShoppingCartSummary current)
    {
        // Find the item to update in our read model
        var itemToUpdate = current.Items.First(i => i.ProductId == @event.ProductId);
        var updatedItems = new List<ShoppingCartSummary.ShoppingCartItemSummary>(current.Items);

        // Create a new summary item with the updated quantity
        updatedItems.Remove(itemToUpdate);
        updatedItems.Add(itemToUpdate with { Quantity = @event.NewQuantity });

        // Recalculate the total price
        var totalPriceAmount = updatedItems.Sum(item => item.Price.Amount * item.Quantity);
        var currency = updatedItems.First().Price.Currency;

        return current with
        {
            Items = updatedItems,
            TotalPrice = new MoneyModel(totalPriceAmount, currency)
        };
    }

    public ShoppingCartSummary Apply(ShoppingCartConfirmed @event, ShoppingCartSummary current)
    {
        return current with
        {
            Status = ShoppingCartStatus.Confirmed.ToString()
        };
    }
}