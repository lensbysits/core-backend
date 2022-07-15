using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace Lens.Core.Lib.Builders
{
    //TODO: add ILogger reference and try to get rid of Serilog dependency
    public interface IApplicationSetupBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
        List<Assembly> Assemblies { get; }
        IHealthChecksBuilder HealthChecks { get; }

        IApplicationSetupBuilder AddAssemblies(params Assembly[] assemblies);
        IApplicationSetupBuilder AddAutoMapper();
        IApplicationSetupBuilder AddMediatR();
    }
}
