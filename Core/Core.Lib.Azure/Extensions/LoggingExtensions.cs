using Lens.Core.Lib.Azure.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Lens.Core.Lib.Azure.Extensions
{
    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithOperationId(this LoggerEnrichmentConfiguration enrichConfiguration)
        {
            if (enrichConfiguration is null) throw new ArgumentNullException(nameof(enrichConfiguration));

            return enrichConfiguration.With<OperationIdEnricher>();
        }

        public static LoggerConfiguration AddAzureAppLogging(this LoggerConfiguration loggerConfiguration, IConfiguration configuration, bool isBootstrap = false)
        {
            var appInsightsConnectionstring = configuration.GetValue<string>("ApplicationInsights:ConnectionString");
            
            if (!string.IsNullOrWhiteSpace(appInsightsConnectionstring))
            {
                var config = TelemetryConfiguration.CreateFromConfiguration(appInsightsConnectionstring);
                return loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithOperationId()
                    .WriteTo.AzureApp()
                    .WriteTo.ApplicationInsights(
                                config, 
                                new OperationTelemetryConverter(), 
                                LogEventLevel.Information); //https://oleh-zheleznyak.blogspot.com/2019/08/serilog-with-application-insights.html
            }

            return loggerConfiguration;
        }
    }
}
