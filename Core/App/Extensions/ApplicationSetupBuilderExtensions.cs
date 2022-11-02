using Lens.Core.App.Services;
using Lens.Core.Lib;
using Lens.Core.Lib.Builders;
using Lens.Core.Lib.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.App;

public static class ApplicationSetupBuilderExtensions
{
    public static IApplicationSetupBuilder AddApplicationServices(this IApplicationSetupBuilder applicationSetup)
    {
        applicationSetup
            .AddAutoMapper()
            .AddMediatR()
            .AddBackgroundTaskQueue()
            .Services
                .AddScoped(typeof(IApplicationService<>), typeof(ApplicationService<>))
                .AddScoped(typeof(IApplicationService<,>), typeof(ApplicationService<,>));

        return applicationSetup;
    }
}