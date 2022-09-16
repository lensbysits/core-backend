using Lens.Core.Data.Attributes;
using Lens.Core.Data.EF.AuditTrail.Entities;
using Lens.Core.Data.Models;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lens.Core.Data.EF.AuditTrail;

public static class DbContextExtensions
{
    private const string NonAuditValue = "[non-audit]";

    public static IEnumerable<EntityChangeModel> CaptureChanges(this DbContext dbContext, IUserContext? userContext = null, string? changeReason = null)
    {
        var changes = new List<EntityChangeModel>();
        var changeTracker = dbContext.ChangeTracker;
        changeTracker.DetectChanges();

        List<EntityState> trackedEntityStates = new() { EntityState.Added, EntityState.Deleted, EntityState.Modified };
        var trackedEntities = changeTracker.Entries().Where(e => trackedEntityStates.Contains(e.State)).ToList();

        var changeToken = Guid.NewGuid();
        foreach (var entry in trackedEntities)
        {
            if (entry.Entity is not IAuditTrailEntity) continue;
            
            changes.Add(LogChanges(entry, userContext, changeReason, changeToken));
        }

        return changes;
    }

    #region Private Static Methods
    private static EntityChangeModel LogChanges(EntityEntry entry, IUserContext? userContext, string? changeReason, Guid changeToken)
    {
        var idPropertyName = entry.Metadata.FindPrimaryKey()?.Properties[0]?.Name ?? "Id";
        
        return new EntityChangeModel
        {
            ChangeType = entry.State.ToString(),
            ChangeReason = changeReason,
            ChangeToken = changeToken,
            ChangedBy = userContext?.Username ?? "anonymous",
            ChangedOn = DateTime.UtcNow,
            EntityType = entry.Metadata.Name,
            EntityId = entry.Property(idPropertyName)?.CurrentValue?.ToString(),
            Changes = GetChanges(entry),
            Source = entry,
            ResolveId = (change) => change.EntityId = entry.Property(idPropertyName)?.CurrentValue?.ToString()
        };
    }

    private static List<EntityChangeProperty?> GetChanges(EntityEntry entry)
    {
        IEnumerable<PropertyEntry>? modifiedProps = null;
        if (entry.State == EntityState.Added)
        {
            modifiedProps = entry.Properties.Where(prop => prop.CurrentValue != null);
        }
        else if (entry.State == EntityState.Deleted)
        {
            modifiedProps = entry.Properties;
        }
        else
        {
            modifiedProps = entry.Properties.Where(prop => prop.IsModified);
        }

        return modifiedProps.Select(prop => GetPropertyChange(prop)).ToList();
    }

    private static EntityChangeProperty? GetPropertyChange(PropertyEntry prop)
    {
        var hasNonAuditAttribute = 
            prop.Metadata.PropertyInfo != null 
            && prop.Metadata.PropertyInfo.GetCustomAttributes(typeof(NonAuditAttribute), false).Any();

        switch (prop.EntityEntry.State)
        {
            case EntityState.Detached:
                break;
            case EntityState.Unchanged:
                break;
            case EntityState.Deleted:
                return new EntityChangeProperty
                {
                    PropertyName = prop.Metadata.Name,
                    OriginalValue = hasNonAuditAttribute ? NonAuditValue : prop.OriginalValue
                };
            case EntityState.Modified:
                return new EntityChangeProperty
                {
                    PropertyName = prop.Metadata.Name,
                    OriginalValue = hasNonAuditAttribute ? NonAuditValue : prop.OriginalValue,
                    NewValue = hasNonAuditAttribute ? NonAuditValue : prop.CurrentValue
                };
            case EntityState.Added:
                return new EntityChangeProperty
                {
                    PropertyName = prop.Metadata.Name,
                    NewValue = hasNonAuditAttribute ? NonAuditValue : prop.CurrentValue
                };
            default:
                break;
        }

        return default;
    }
    #endregion
}
