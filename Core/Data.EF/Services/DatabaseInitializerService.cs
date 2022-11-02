using Lens.Core.Data.EF.Configuration;
using Lens.Core.Data.EF.Providers;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Lens.Core.Data.EF.Services;

public abstract class DatabaseInitializerService : BaseService<DatabaseInitializerService>, IProgramInitializer
{
    private readonly DbContext _dbContext;
    private readonly MigrationSettings options;

    public DatabaseInitializerService(IApplicationService<DatabaseInitializerService> applicationService,
        DbContext dbContext,
        IOptions<MigrationSettings>? options = null)
        : base(applicationService)
    {
        _dbContext = dbContext;
        this.options = options?.Value ?? new MigrationSettings();
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
                foreach (var command in commands)
                {
                    await _dbContext.Database.ExecuteSqlRawAsync(command);
                }
            }
        }
        catch (Exception ex)
        {
            LogException(ex);

            if (this.options.BreakOnMigrationException)
            {
                throw new ApiStartupException("Migrations failed. See logs for details");
            }
            else
            {
                return;
            }
        }

        await Seed();
    }

    private void LogException(Exception ex)
    {
        ApplicationService.Logger.LogError(ex, "An error had occured when applying db migrations.");

        if (this.options.EnableRawSqlDebug)
        {
            var sb = new StringBuilder();
            foreach (var cmd in RawSqlProvider.Instance.GetSqlCommands())
            {
                sb.AppendLine(cmd);
                sb.AppendLine(string.Empty.PadLeft(100, '-'));
            }

            ApplicationService.Logger.LogDebug($"Raw sql debug log:\n{sb}");
        }
    }

    public async virtual Task Seed()
    {
        await Task.FromResult(0);
    }
}

public class DatabaseInitializerService<TDbContext> : DatabaseInitializerService where TDbContext : DbContext
{
    public DatabaseInitializerService(
        IApplicationService<DatabaseInitializerService> applicationService,
        TDbContext dbContext,
        IOptions<MigrationSettings> options)
        : base(applicationService, dbContext, options)
    {
    }
}