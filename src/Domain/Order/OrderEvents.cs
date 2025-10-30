using Domain.SharedKernel;

namespace Domain.Order;

public record OrderPlaced(
    Guid OrderId,
    Guid CustomerId,
    IReadOnlyList<OrderLine> OrderLines,
    Money TotalPrice,
    DateTime PlacedAt
) : DomainEvent;