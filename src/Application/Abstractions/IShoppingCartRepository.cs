using SC = Domain.ShoppingCart;

namespace Application.Abstractions;

public interface IShoppingCartRepository
{
    Task<SC.ShoppingCart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(SC.ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
    Task UpdateAsync(SC.ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
}