using Lens.Core.Lib.Builders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Azure.Extensions
{
    public static class ApplicationSetupBuilderExtensions
    {
        public static IApplicationSetupBuilder AddAzureApplicationsInsightsForWeb(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup.Services.AddApplicationInsightsTelemetry(applicationSetup.Configuration);

            return applicationSetup;
        }

        public static IApplicationSetupBuilder AddAzureApplicationsInsightsForWorkers(this IApplicationSetupBuilder applicationSetup)
        {
            applicationSetup.Services.AddApplicationInsightsTelemetryWorkerService(applicationSetup.Configuration);

            return applicationSetup;
        }
    }
}
