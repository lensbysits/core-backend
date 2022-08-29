using Lens.Core.Data.Models;

namespace Lens.Core.Data.Services;

public interface IAuditTrailService
{
    Task LogChanges(Func<IEnumerable<EntityChangeModel>> changes);
    Task<IEnumerable<EntityChangeModel>> GetChanges<TEntity>(string id);
}
