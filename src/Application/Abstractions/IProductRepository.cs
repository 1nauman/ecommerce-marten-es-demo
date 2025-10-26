using DP = Domain.Product;

namespace Application.Abstractions;

public interface IProductRepository
{
    Task<DP.Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(DP.Product product, CancellationToken cancellationToken = default);
}