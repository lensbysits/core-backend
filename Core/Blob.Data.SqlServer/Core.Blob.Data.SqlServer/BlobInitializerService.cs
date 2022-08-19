using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lens.Core.Blob.Data;

public class BlobInitializerService : BaseService<BlobInitializerService>, IProgramInitializer
{
    protected readonly BlobDbContext _dbContext;

    public BlobInitializerService(IApplicationService<BlobInitializerService> applicationService,
        BlobDbContext dbContext)
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
            ApplicationService.Logger.LogError(ex, "An error had occured when applying blob db migrations.");
            return;
        }
    }
}
