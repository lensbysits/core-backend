using CoreLib.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreLib.Services
{
    public interface IAuditTrailService
    {
        Task LogChanges(Func<IEnumerable<EntityChangeModel>> changes);
        Task<IEnumerable<EntityChangeModel>> GetChanges<TEntity>(string id);
    }
}
