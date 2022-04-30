using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lens.Core.Data.EF.Services
{
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
                await _dbContext.Database.MigrateAsync();
            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //    ApplicationService.Logger.LogError(ex, "An error had occured when applying db migrations.");
            //    return;
            //}

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
}