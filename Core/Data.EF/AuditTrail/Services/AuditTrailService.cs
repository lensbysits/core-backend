using Lens.Core.Data.EF.AuditTrail.Entities;
using Lens.Core.Data.Models;
using Lens.Core.Data.Services;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lens.Core.Data.EF.AuditTrail.Services;

public class AuditTrailService : BaseService<AuditTrailService>, IAuditTrailService
{
    private readonly AuditTrailDbContext _dbContext;

    public AuditTrailService(
        IApplicationService<AuditTrailService> applicationService,
        AuditTrailDbContext dbContext) : base(applicationService)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<EntityChangeModel>> GetChanges<TEntity>(string id)
    {
        var changes = await _dbContext.EntityChanges
            .Where(c => c.EntityId == id && c.EntityType == typeof(TEntity).FullName)
            .ToListAsync();

        var result = ApplicationService.Mapper.Map<IEnumerable<EntityChangeModel>>(changes);
        return result;
    }

    public async Task LogChanges(Func<IEnumerable<EntityChangeModel>> changes)
    {
        var changeList = changes();
        if (changeList == null) return;

        foreach (var item in changeList)
        {
            var dbItem = ApplicationService.Mapper.Map<EntityChange>(item);
            _dbContext.EntityChanges.Add(dbItem);
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            ApplicationService.Logger.LogError(e, "Something went wrong saving audit-trail");
        }
    }
}
