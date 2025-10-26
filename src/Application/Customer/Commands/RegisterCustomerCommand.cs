using Application.Abstractions;
using MediatR;
using CD = Domain.Customer;

namespace Application.Customer.Commands;

public record RegisterCustomerCommand(string Name, string Email) : IRequest<Guid>;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, Guid>
{
    private readonly ICustomerRepository _repository;

    public RegisterCustomerCommandHandler(ICustomerRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public async Task<Guid> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = CD.Customer.Register(request.Name, request.Email);
        await _repository.AddAsync(customer, cancellationToken);
        return customer.Id;
    }
}