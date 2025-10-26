using Application.Abstractions;
using Domain.Product;
using Marten;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDocumentSession _session;

    public ProductRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.Events.AggregateStreamAsync<Product>(id, token: cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _session.Events.StartStream<Product>(product.Id, product.GetUncommittedEvents());
        await _session.SaveChangesAsync(cancellationToken);
    }
}