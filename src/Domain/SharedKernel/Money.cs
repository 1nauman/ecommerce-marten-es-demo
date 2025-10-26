namespace Domain.SharedKernel;

public record struct Money
{
    public static Money Invalid = new Money(0, "INV");

    public decimal Amount { get; }
    public string Currency { get; }

    // --- THIS IS THE FIX ---
    // The constructor is now public. This allows the JSON serializer (System.Text.Json)
    // to create instances of the Money object when deserializing events from the database.
    // Our validation logic remains intact, ensuring no invalid Money can ever be created.
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Money amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentNullException(nameof(currency), "Currency cannot be null or empty.");
        if (currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter ISO code.", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpper();
    }

    // The static factory method is still useful for semantic clarity,
    // but the public constructor is what makes serialization work.
    public static Money Create(decimal amount, string currency)
    {
        return new Money(amount, currency);
    }

    public override string ToString() => $"{Amount} {Currency}";
}