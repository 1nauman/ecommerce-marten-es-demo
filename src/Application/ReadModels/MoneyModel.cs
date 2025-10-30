using Domain.SharedKernel;

namespace Application.ReadModels;

/// <summary>
/// A simple Data Transfer Object (DTO) for representing monetary values in read models.
/// </summary>
public record MoneyModel
{
    public static MoneyModel Invalid => new(0, string.Empty);

    public MoneyModel(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;

    public static MoneyModel FromDomain(Money money) => new(money.Amount, money.Currency);
    
    public Money ToDomain() => Money.Create(Amount, Currency);
}