using Application.Abstractions;
using MediatR;
using SC = Domain.ShoppingCart;

namespace Application.ShoppingCart.Commands;

// Command definition for opening a shopping cart
public record OpenShoppingCartCommand(Guid CustomerId) : IRequest<Guid>;

public class OpenShoppingCartCommandHandler : IRequestHandler<OpenShoppingCartCommand, Guid>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public OpenShoppingCartCommandHandler(IShoppingCartRepository shoppingCartRepository)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartRepository);

        _shoppingCartRepository = shoppingCartRepository;
    }

    public async Task<Guid> Handle(OpenShoppingCartCommand request, CancellationToken cancellationToken)
    {
        var shoppingCart = SC.ShoppingCart.Open(request.CustomerId);

        await _shoppingCartRepository.AddAsync(shoppingCart, cancellationToken);

        return shoppingCart.Id;
    }
}