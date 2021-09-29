namespace Lens.Core.Lib.Services
{
    public abstract class BaseService<TLogger>
    {
        protected readonly IApplicationService<TLogger> ApplicationService;

        public BaseService(IApplicationService<TLogger> applicationService)
        {
            ApplicationService = applicationService;
        }
    }

    public abstract class BaseService<TLogger, TSettings> where TSettings : class
    {
        protected readonly IApplicationService<TLogger, TSettings> ApplicationService;

        public BaseService(IApplicationService<TLogger, TSettings> applicationService)
        {
            ApplicationService = applicationService;
        }
    }
}
