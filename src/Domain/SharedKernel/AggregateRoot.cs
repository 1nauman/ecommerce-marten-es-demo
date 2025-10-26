namespace Domain.SharedKernel;

/// <summary>
/// Represents the base class for all event-sourced aggregates.
/// It provides the core, shared functionality for tracking, applying, and raising domain events.
/// </summary>
public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }
    public int Version { get; protected set; }

    private readonly List<DomainEvent> _uncommittedEvents = new();

    /// <summary>
    /// Gets the collection of events that have been raised but not yet committed to the event store.
    /// </summary>
    public IReadOnlyCollection<DomainEvent> GetUncommittedEvents() => _uncommittedEvents;

    /// <summary>
    /// Clears the list of uncommitted events. This should be called after the events have been successfully persisted.
    /// </summary>
    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    /// <summary>
    /// The primary method for a derived aggregate to signal that a new domain event has occurred.
    /// This method applies the event to the aggregate and adds it to the uncommitted events list.
    /// </summary>
    /// <param name="event">The new domain event that has occurred.</param>
    protected void Raise(DomainEvent @event)
    {
        // Apply the state change to the aggregate first.
        ApplyChange(@event);

        // Then, add it to the list of new events to be persisted.
        _uncommittedEvents.Add(@event);
    }

    /// <summary>
    /// Reconstructs the aggregate's current state by applying a sequence of historical events.
    /// This method does not add the events to the uncommitted list.
    /// </summary>
    /// <param name="history">An ordered collection of historical domain events.</param>
    public void LoadFromHistory(IEnumerable<DomainEvent> history)
    {
        foreach (var e in history)
        {
            // Apply the state change but do not track it as a new event.
            ApplyChange(e);
        }
    }

    /// <summary>
    /// The central, private orchestrator for applying an event's state change.
    /// It ensures the correct handler is called and the aggregate's version is incremented.
    /// </summary>
    private void ApplyChange(DomainEvent @event)
    {
        // Dispatch the event to the appropriate, strongly-typed handler.
        Dispatch(@event);

        // Increment the version after every successfully applied event.
        Version++;
    }

    // --- IMPORTANT ---
    // This is the "bridge" that connects the generic logic of the AggregateRoot
    // to the specific, private event handlers of the derived aggregate.
    // The derived aggregate MUST implement this method.

    /// <summary>
    /// Dispatches the event to the correct, private, strongly-typed "Apply" method
    /// within the derived aggregate using dynamic dispatch.
    /// </summary>
    /// <param name="event">The domain event to be dispatched.</param>
    protected abstract void Dispatch(DomainEvent @event);
}