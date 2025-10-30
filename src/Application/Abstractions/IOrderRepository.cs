namespace Application.Abstractions;

public interface IOrderRepository
{
    Task AddAsync(Domain.Order.Order order, CancellationToken cancellationToken = default);
}