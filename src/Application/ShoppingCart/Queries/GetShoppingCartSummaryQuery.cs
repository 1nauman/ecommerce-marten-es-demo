using Application.Abstractions;
using Application.ReadModels;
using MediatR;

namespace Application.ShoppingCart.Queries;

// The query record - it takes the ID of the cart we want to find.
public record GetShoppingCartSummaryQuery(
    Guid ShoppingCartId
) : IRequest<ShoppingCartSummary?>; // It returns our nullable read model.

// The handler for the query.
public class GetShoppingCartSummaryQueryHandler : IRequestHandler<GetShoppingCartSummaryQuery, ShoppingCartSummary?>
{
    private readonly IShoppingCartReadRepository _shoppingCartReadRepository;

    // We inject Marten's IQuerySession, which is a lightweight, read-only session.
    public GetShoppingCartSummaryQueryHandler(IShoppingCartReadRepository shoppingCartReadRepository)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartReadRepository);
        _shoppingCartReadRepository = shoppingCartReadRepository;
    }

    public async Task<ShoppingCartSummary?> Handle(GetShoppingCartSummaryQuery request,
        CancellationToken cancellationToken)
    {
        // Use the session's LoadAsync method to fetch the read model document by its ID.
        var summary = await _shoppingCartReadRepository.GetSummaryAsync(request.ShoppingCartId, cancellationToken);

        return summary;
    }
}