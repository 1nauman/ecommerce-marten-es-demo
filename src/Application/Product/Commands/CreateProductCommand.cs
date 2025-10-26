using Application.Abstractions;
using Application.ReadModels; // For MoneyModel
using Domain.SharedKernel;
using MediatR;

namespace Application.Product.Commands;

public record CreateProductCommand(
    string Name,
    MoneyModel Price
) : IRequest<Guid>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var price = Money.Create(request.Price.Amount, request.Price.Currency);
        var product = Domain.Product.Product.Create(request.Name, price);

        await _productRepository.AddAsync(product, cancellationToken);

        return product.Id;
    }
}