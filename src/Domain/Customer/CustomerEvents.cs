using Domain.SharedKernel;

namespace Domain.Customer;

public record CustomerRegistered(
    Guid CustomerId,
    string Name,
    string Email
) : DomainEvent;

public record ShippingAddressUpdated(
    Guid CustomerId,
    Address ShippingAddress
) : DomainEvent;