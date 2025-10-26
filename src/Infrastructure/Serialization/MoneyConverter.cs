using Domain.SharedKernel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Serialization;

/// <summary>
/// A custom JsonConverter for the domain's Money value object.
/// This allows the pure domain object to be correctly serialized and deserialized
/// without needing any infrastructure-specific attributes.
/// </summary>
public class MoneyConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        decimal amount = 0;
        string currency = string.Empty;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new Money(amount, currency);
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString()?.ToLowerInvariant();
                reader.Read();

                switch (propertyName)
                {
                    case "amount":
                        amount = reader.GetDecimal();
                        break;
                    case "currency":
                        currency = reader.GetString() ?? string.Empty;
                        break;
                }
            }
        }
        throw new JsonException("Error reading Money JSON.");
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("amount", value.Amount);
        writer.WriteString("currency", value.Currency);
        writer.WriteEndObject();
    }
}