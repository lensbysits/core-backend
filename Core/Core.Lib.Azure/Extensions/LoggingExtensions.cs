using Lens.Core.Lib.Azure.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Azure.Extensions
{
    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithOperationId(this LoggerEnrichmentConfiguration enrichConfiguration)
        {
            if (enrichConfiguration is null) throw new ArgumentNullException(nameof(enrichConfiguration));

            return enrichConfiguration.With<OperationIdEnricher>();
        }

        public static LoggerConfiguration AddAzureAppLogging(this LoggerConfiguration loggerConfiguration, bool isBootstrap = false)
        {
            return loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithOperationId()
                .WriteTo.AzureApp()
                .WriteTo.ApplicationInsights(new OperationTelemetryConverter(), LogEventLevel.Information); //https://oleh-zheleznyak.blogspot.com/2019/08/serilog-with-application-insights.html
        }
    }
}
