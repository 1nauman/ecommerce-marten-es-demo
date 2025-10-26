using Application.ReadModels;

namespace Application.Abstractions;

/// <summary>
/// Defines the contract for a repository that handles read-only queries for shopping cart summaries.
/// This abstraction belongs to the Application layer.
/// </summary>
public interface IShoppingCartReadRepository
{
    Task<ShoppingCartSummary?> GetSummaryAsync(Guid id, CancellationToken cancellationToken = default);
}