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
            var appInsightsConnectionstring = configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");

            if (!string.IsNullOrWhiteSpace(appInsightsConnectionstring))
            {
                var config = TelemetryConfiguration.CreateFromConfiguration(appInsightsConnectionstring);
                config.InstrumentationKey = ParseFromConnectionString(appInsightsConnectionstring);

                return loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithOperationId()
                    .WriteTo.AzureApp()
                    .WriteTo.ApplicationInsights(
                                config,
                                new OperationTelemetryConverter()); //https://oleh-zheleznyak.blogspot.com/2019/08/serilog-with-application-insights.html
            }

            return loggerConfiguration;
        }

        /// <summary>
        /// Serilog doesn't seems to handle the telemetry config created from the connectionstring properly
        /// So we need to set the deprecated instrumentation key to get serilog to work.
        /// Sould be remove as soon as serilog also has upgraded the the application insights connection string.
        /// </summary>
        /// <param name="appInsightsConnectionstring">The application insights connectionstring.</param>
        /// <returns>Instrumentation key</returns>
        private static string ParseFromConnectionString(string appInsightsConnectionstring)
        {
            var parts = appInsightsConnectionstring.Split(';');
            var part = parts.FirstOrDefault(p => p.StartsWith("InstrumentationKey="));
            var key = part?.Split('=')[1];

            return key ?? string.Empty;
        }
    }
}
