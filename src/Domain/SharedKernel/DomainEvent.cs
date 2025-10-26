namespace Domain.SharedKernel;

/// <summary>
/// An abstract base record for all domain events.
/// It includes common properties that every event should have.
/// </summary>
public abstract record DomainEvent
{
    /// <summary>
    /// The UTC timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}