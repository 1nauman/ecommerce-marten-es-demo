namespace Application.ReadModels;

// This is the document we will query. It's optimized for reading.
public record ShoppingCartSummary
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string Status { get; init; } = string.Empty;
    public List<ShoppingCartItemSummary> Items { get; init; } = new();
    public MoneyModel TotalPrice { get; init; } = MoneyModel.Invalid;

    // A nested record for the items in the summary
    public record ShoppingCartItemSummary
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public MoneyModel Price { get; init; } = MoneyModel.Invalid;
    }
}