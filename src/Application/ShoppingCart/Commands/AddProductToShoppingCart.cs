using Application.Abstractions;

using MediatR;

namespace Application.ShoppingCart.Commands;

public record AddProductToShoppingCart(Guid ShoppingCartId, Guid ProductId, int Quantity) : IRequest;

public class AddProductToShoppingCartHandler : IRequestHandler<AddProductToShoppingCart>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IProductRepository _productRepository;

    public AddProductToShoppingCartHandler(IShoppingCartRepository shoppingCartRepository,
        IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartRepository);
        ArgumentNullException.ThrowIfNull(productRepository);

        _shoppingCartRepository = shoppingCartRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(AddProductToShoppingCart request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new InvalidOperationException($"Product not found for Id: {request.ProductId}.");
        }

        var shoppingCart = await _shoppingCartRepository.GetByIdAsync(request.ShoppingCartId, cancellationToken);
        if (shoppingCart is null)
        {
            throw new InvalidOperationException("Shopping cart not found.");
        }

        shoppingCart.AddProduct(product.Id, request.Quantity, product.Price);

        await _shoppingCartRepository.UpdateAsync(shoppingCart, cancellationToken);
    }
}