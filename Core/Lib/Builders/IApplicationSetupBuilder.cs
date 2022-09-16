using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Lens.Core.Lib.Builders;

public interface IApplicationSetupBuilder
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
    List<Assembly> Assemblies { get; }

    IApplicationSetupBuilder AddAssemblies(params Assembly[] assemblies);
    IApplicationSetupBuilder AddAutoMapper();
    IApplicationSetupBuilder AddMediatR();
}
