namespace Lens.Core.Data.Models;

public class EntityChangeProperty
{
    public string? PropertyName { get; set; }
    public object? OriginalValue { get; set; }
    public object? NewValue { get; set; }
}
