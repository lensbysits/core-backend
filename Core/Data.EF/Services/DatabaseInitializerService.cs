using Lens.Core.Data.EF.Providers;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lens.Core.Data.EF.Services;

public abstract class DatabaseInitializerService : BaseService<DatabaseInitializerService>, IProgramInitializer
{
    private readonly DbContext _dbContext;

    public DatabaseInitializerService(IApplicationService<DatabaseInitializerService> applicationService,
        DbContext dbContext)
        : base(applicationService)
    {
        _dbContext = dbContext;
    }

    public async Task Initialize()
    {
        try
        {
            var pending = await _dbContext.Database.GetPendingMigrationsAsync();
            await _dbContext.Database.MigrateAsync();
            
            // only apply raw sql when there are pending changes to prevent raw sql running everytime the API starts
            if (pending.ToList().Count > 0)
            {
                var commands = RawSqlProvider.Instance.GetSqlCommands();
                foreach(var command in commands)
                {
                    await _dbContext.Database.ExecuteSqlRawAsync(command);
                }
            }
        }
        catch (Exception ex)
        {
            ApplicationService.Logger.LogError(ex, "An error had occured when applying db migrations.");
            return;
        }

        await Seed();
    }

    public async virtual Task Seed()
    {
        await Task.FromResult(0);
    }
}

public class DatabaseInitializerService<TDbContext> : DatabaseInitializerService where TDbContext: DbContext
{
    public DatabaseInitializerService(IApplicationService<DatabaseInitializerService> applicationService, TDbContext dbContext) : base(applicationService, dbContext)
    {
    }
}