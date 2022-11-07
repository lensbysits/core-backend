using Lens.Core.App.Web.Options;
using Lens.Core.Lib.Builders;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.App.Web.Builders;

public interface IWebApplicationSetupBuilder : IApplicationSetupBuilder
{
    IControllerOptions Controller { get; }
    IHealthChecksBuilder HealthChecks { get; }
    ICollection<Type> RequestPipelineFilterMetadata { get; }

}