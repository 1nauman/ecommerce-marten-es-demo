using Application.Abstractions;
using MediatR;

namespace Application.ShoppingCart.Commands;

// The command only needs the ID of the cart to confirm.
public record ConfirmShoppingCartCommand(Guid ShoppingCartId) : IRequest;

public class ConfirmShoppingCartCommandHandler : IRequestHandler<ConfirmShoppingCartCommand>
{
    private readonly IShoppingCartRepository _repository;

    public ConfirmShoppingCartCommandHandler(IShoppingCartRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ConfirmShoppingCartCommand request, CancellationToken cancellationToken)
    {
        // 1. Load the aggregate
        var cart = await _repository.GetByIdAsync(request.ShoppingCartId, cancellationToken);
        if (cart is null)
            throw new Exception("Shopping cart not found.");

        // 2. Execute the domain behavior
        cart.Confirm();

        // 3. Persist the changes
        await _repository.UpdateAsync(cart, cancellationToken);
    }
}