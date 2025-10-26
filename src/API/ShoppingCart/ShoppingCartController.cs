using Application.ShoppingCart.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.ShoppingCart;

[ApiController]
[Route("api/shopping-carts")]
public class ShoppingCartController : Controller
{
    private readonly ISender _sender;

    public ShoppingCartController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> OpenShoppingCart([FromBody] OpenShoppingCartRequest request)
    {
        var command = new OpenShoppingCartCommand(request.CustomerId);

        var shoppingCartId = await _sender.Send(command);

        return Ok(shoppingCartId);
    }

    [HttpPost("{shoppingCartId:guid}/products")]
    public async Task<IActionResult> AddProduct([FromRoute] Guid shoppingCartId, [FromBody] AddProductRequest request)
    {
        var command = new AddProductToShoppingCart(shoppingCartId, request.ProductId, request.Quantity);

        await _sender.Send(command);

        return NoContent();
    }
}

// This is a Data Transfer Object (DTO) used to bind the incoming JSON request.
public record OpenShoppingCartRequest(Guid CustomerId);

public record AddProductRequest(Guid ProductId, int Quantity);