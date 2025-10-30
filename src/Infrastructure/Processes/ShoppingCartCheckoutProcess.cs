namespace Infrastructure.Processes;

/// <summary>
/// A stateful document used to track the progress of the Cart-to-Order workflow.
/// This ensures the process is idempotent and provides an audit trail.
/// </summary>
public class ShoppingCartCheckoutProcess
{
    // Marten requires a public Id property for documents.
    public Guid Id { get; init; } = Guid.NewGuid();

    // We'll use the CartId as the unique identifier for this process.
    public Guid CartId { get; set; }
    public Guid? OrderId { get; set; }
    public ProcessStatus Status { get; set; }
    public string? ErrorMessage { get; set; }

    // This is how Marten can find an existing process document.
    // We are telling it that the CartId property is an alternate key.
    [Marten.Schema.Identity] public Guid MartenId => CartId;
}

public enum ProcessStatus
{
    Started,
    OrderPlaced,
    Completed,
    Failed
}