using Application.Abstractions;
using Domain.SharedKernel;
using MediatR;

namespace Application.ShoppingCart.Commands;

public record AddProductToShoppingCart(Guid ShoppingCartId, Guid ProductId, int Quantity) : IRequest;

public class AddProductToShoppingCartHandler : IRequestHandler<AddProductToShoppingCart>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public AddProductToShoppingCartHandler(IShoppingCartRepository shoppingCartRepository)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartRepository);
        _shoppingCartRepository = shoppingCartRepository;
    }

    public async Task Handle(AddProductToShoppingCart request, CancellationToken cancellationToken)
    {
        var shoppingCart = await _shoppingCartRepository.GetByIdAsync(request.ShoppingCartId, cancellationToken);

        if (shoppingCart is null)
        {
            throw new InvalidOperationException("Shopping cart not found.");
        }
        
        var productPrice = Money.Create(100, "USD");
        
        shoppingCart.AddProduct(request.ProductId, request.Quantity, productPrice);
        
        await _shoppingCartRepository.UpdateAsync(shoppingCart, cancellationToken);
    }
}