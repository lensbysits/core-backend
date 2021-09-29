using CoreLib.Models;
using CoreLib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreApp.Data.AuditTrail
{
    public static class DbContextExtensions
    {
        public static IEnumerable<EntityChangeModel> CaptureChanges(this DbContext dbContext, IUserContext userContext = null, string changeReason = null)
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
        private static EntityChangeModel LogChanges(EntityEntry entry, IUserContext userContext, string changeReason, Guid changeToken)
        {
            string propertyName = entry.Metadata.FindPrimaryKey().Properties[0].Name;
            
            return new EntityChangeModel
            {
                ChangeType = entry.State.ToString(),
                ChangeReason = changeReason,
                ChangeToken = changeToken,
                ChangedBy = userContext?.Username ?? "anonymous",
                ChangedOn = DateTime.UtcNow,
                EntityType = entry.Metadata.Name,
                EntityId = entry.Property(propertyName)?.CurrentValue?.ToString(),
                Changes = GetChanges(entry),
                Source = entry,
                ResolveId = (change) => change.EntityId = entry.Property(propertyName)?.CurrentValue?.ToString()
            };
        }

        private static List<EntityChangeProperty> GetChanges(EntityEntry entry)
        {
            IEnumerable<PropertyEntry> modifiedProps = null;
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

        private static EntityChangeProperty GetPropertyChange(PropertyEntry prop)
        {
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
                        OriginalValue = prop.OriginalValue
                    };
                case EntityState.Modified:
                    return new EntityChangeProperty
                    {
                        PropertyName = prop.Metadata.Name,
                        OriginalValue = prop.OriginalValue,
                        NewValue = prop.CurrentValue
                    };
                case EntityState.Added:
                    return new EntityChangeProperty
                    {
                        PropertyName = prop.Metadata.Name,
                        NewValue = prop.CurrentValue
                    };
                default:
                    break;
            }

            return default;
        }
        #endregion
    }
}
