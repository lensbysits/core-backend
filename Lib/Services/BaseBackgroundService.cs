using Microsoft.Extensions.Hosting;

namespace Lens.Core.Lib.Services
{
    public abstract class BaseBackgroundService<TLogger> : BackgroundService
    {
        protected readonly IApplicationService<TLogger> ApplicationService;

        public BaseBackgroundService(IApplicationService<TLogger> applicationService)
        {
            ApplicationService = applicationService;
        }
    }

    public abstract class BaseBackgroundService<TLogger, TSettings> : BackgroundService where TSettings : class
    {
        protected readonly IApplicationService<TLogger, TSettings> ApplicationService;

        public BaseBackgroundService(IApplicationService<TLogger, TSettings> applicationService)
        {
            ApplicationService = applicationService;
        }
    }
}
