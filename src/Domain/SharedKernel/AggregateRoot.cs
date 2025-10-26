namespace Domain.SharedKernel;

public abstract class AggregateRoot
{
    private readonly List<DomainEvent> _uncommittedEvents = [];

    public Guid Id { get; protected set; }

    public int Version { get; protected set; }

    public IReadOnlyCollection<DomainEvent> UncommittedEvents => _uncommittedEvents;

    protected void Raise(DomainEvent @event)
    {
        // Apply the state change
        Apply(@event);
        // Add to the list of changes to be persisted
        _uncommittedEvents.Add(@event);
    }

    protected virtual void Apply(DomainEvent @event)
    {
        // Dynamically invoke the correct private Apply method.
        ((dynamic)this).Apply((dynamic)@event);
        Version++;
    }

    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    public void LoadFromHistory(IEnumerable<DomainEvent> history)
    {
        foreach (var e in history)
        {
            // Apply the state change but do not add to uncommitted events
            Apply(e);
        }
    }
}