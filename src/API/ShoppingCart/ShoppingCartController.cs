using Application.ShoppingCart.Commands;
using Application.ShoppingCart.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace API.ShoppingCart;

[ApiController]
[Route("api/shopping-carts")]
public class ShoppingCartController : ControllerBase
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

    [HttpGet("{shoppingCartId:guid}")]
    public async Task<IActionResult> GetShoppingCartSummary([FromRoute] Guid shoppingCartId)
    {
        var query = new GetShoppingCartSummaryQuery(shoppingCartId);

        var summary = await _sender.Send(query);

        return summary is not null ? Ok(summary) : NotFound();
    }

    [HttpPut("{shoppingCartId:guid}/confirm")]
    public async Task<IActionResult> ConfirmShoppingCart([FromRoute] Guid shoppingCartId)
    {
        var command = new ConfirmShoppingCartCommand(shoppingCartId);

        await _sender.Send(command);

        return NoContent();
    }
}

// This is a Data Transfer Object (DTO) used to bind the incoming JSON request.
public record OpenShoppingCartRequest(Guid CustomerId);

public record AddProductRequest(Guid ProductId, int Quantity);