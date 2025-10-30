using Application.Abstractions;

using Domain.Order;

using Marten;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IDocumentSession _session;

    public OrderRepository(IDocumentSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _session.Events.StartStream<Order>(order.Id, order.GetUncommittedEvents());
        await _session.SaveChangesAsync(cancellationToken);
    }
}