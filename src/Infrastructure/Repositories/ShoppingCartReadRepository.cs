using Application.Abstractions;
using Application.ReadModels;
using Marten;

namespace Infrastructure.Repositories;

public class ShoppingCartReadRepository : IShoppingCartReadRepository
{
    private readonly IQuerySession _querySession;

    public ShoppingCartReadRepository(IQuerySession querySession)
    {
        ArgumentNullException.ThrowIfNull(querySession);
        _querySession = querySession;
    }

    public async Task<ShoppingCartSummary?> GetSummaryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _querySession.LoadAsync<ShoppingCartSummary>(id, cancellationToken);
    }
}