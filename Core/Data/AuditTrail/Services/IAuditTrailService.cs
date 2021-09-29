using Lens.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lens.Core.Data.Services
{
    public interface IAuditTrailService
    {
        Task LogChanges(Func<IEnumerable<EntityChangeModel>> changes);
        Task<IEnumerable<EntityChangeModel>> GetChanges<TEntity>(string id);
    }
}
