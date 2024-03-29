﻿using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lens.Core.Data.EF.AuditTrail.Services;

public class AuditTrailInitializerService : BaseService<AuditTrailInitializerService>, IProgramInitializer
{
    protected readonly AuditTrailDbContext _dbContext;

    public AuditTrailInitializerService(IApplicationService<AuditTrailInitializerService> applicationService,
        AuditTrailDbContext dbContext)
        : base(applicationService)
    {
        _dbContext = dbContext;
    }

    public async Task Initialize()
    {
        try
        {
            await _dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            ApplicationService.Logger.LogError(ex, "An error had occured when applying audittrail db migrations.");
            return;
        }
    }
}
