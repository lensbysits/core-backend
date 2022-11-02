namespace Lens.Core.Data.Models;

public class EntityChangeModel
{
    public string? ChangeType { get; set; }
    public string? ChangeReason { get; set; }
    public Guid ChangeToken { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedOn { get; set; }
    public IEnumerable<EntityChangeProperty?>? Changes { get; set; }

    public string? EntityType { get; set; }
    public string? EntityId { get; set; }

    public object? Source { get; set; }
    public Action<EntityChangeModel>? ResolveId { get; set; }
    public Guid? CorrelationId { get; set; }
}
