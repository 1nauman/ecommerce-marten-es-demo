using Application.Abstractions;
using Domain.Customer;
using Marten;

namespace Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDocumentSession _session;

    public CustomerRepository(IDocumentSession session)
    {
        _session = session;
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _session.Events.AggregateStreamAsync<Customer>(id, token: cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _session.Events.StartStream<Customer>(customer.Id, customer.GetUncommittedEvents());
        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _session.Events.AppendOptimistic(customer.Id, cancellationToken,
            customer.GetUncommittedEvents().ToArray<object>());
        await _session.SaveChangesAsync(cancellationToken);
    }
}