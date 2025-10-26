using Domain.SharedKernel;

namespace Domain.Product;

public record ProductCreated(
    Guid ProductId,
    string Name,
    Money Price
) : DomainEvent;