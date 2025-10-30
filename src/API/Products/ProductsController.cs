using Application.Product.Commands;
using Application.ReadModels;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace API.Products;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var command = new CreateProductCommand(request.Name, request.Price);
        var productId = await _sender.Send(command);
        return Ok(productId);
    }
}

public record CreateProductRequest(string Name, MoneyModel Price);