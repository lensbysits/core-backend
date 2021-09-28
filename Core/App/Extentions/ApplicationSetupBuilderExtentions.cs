using CoreApp.Services;
using CoreLib.Builders;
using CoreLib.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApp
{
    public static class ApplicationSetupBuilderExtentions
    {
        public static IApplicationSetupBuilder AddApplicationServices(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup.Services
                .AddAutoMapper(applicationSetup.Assemblies)
                .AddMediatR(applicationSetup.Assemblies.ToArray())

                .AddScoped(typeof(IApplicationService<>), typeof(ApplicationService<>))
                .AddScoped(typeof(IApplicationService<,>), typeof(ApplicationService<,>));


            return applicationSetup;
        }
    }
}