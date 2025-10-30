using Domain.SharedKernel;

namespace Domain.Order;

public record OrderLine(Guid ProductId, int Quantity, Money Price);