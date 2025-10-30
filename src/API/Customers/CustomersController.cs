using Application.Customer.Commands;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace API.Customers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest request)
    {
        var command = new RegisterCustomerCommand(request.Name, request.Email);
        var customerId = await _sender.Send(command);
        return Ok(customerId);
    }
}

public record RegisterCustomerRequest(string Name, string Email);