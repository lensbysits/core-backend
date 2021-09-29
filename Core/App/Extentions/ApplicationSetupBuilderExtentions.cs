using CoreApp.Services;
using CoreLib.Builders;
using CoreLib.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApp
{
    public static class ApplicationSetupBuilderExtentions
    {
        public static IApplicationSetupBuilder AddApplicationServices(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup
                .AddAutoMapper()
                .AddMediatR()
            .Services
                .AddScoped(typeof(IApplicationService<>), typeof(ApplicationService<>))
                .AddScoped(typeof(IApplicationService<,>), typeof(ApplicationService<,>));

            return applicationSetup;
        }
    }
}