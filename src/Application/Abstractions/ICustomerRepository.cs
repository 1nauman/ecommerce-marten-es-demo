using CD = Domain.Customer;

namespace Application.Abstractions;

public interface ICustomerRepository
{
    Task<CD.Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(CD.Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(CD.Customer customer, CancellationToken cancellationToken = default);
}