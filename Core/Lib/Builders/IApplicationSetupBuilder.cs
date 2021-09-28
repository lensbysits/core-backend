using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace CoreLib.Builders
{
    public interface IApplicationSetupBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
        List<Assembly> Assemblies { get; }

        IApplicationSetupBuilder AddAssemblies(params Assembly[] assemblies);
    }
}
