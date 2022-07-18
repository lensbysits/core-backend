using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Azure.Logging
{
    public class AzureAppSink : ILogEventSink
    {
        /// <summary>
        /// Our very own Microsoft.Extensions.LoggerFactory, this is where we'll send Serilog events so that Azure can pick up the logs.
        /// We expect that Serilog has replaced this in the app's services.
        /// </summary>
        static ILoggerFactory CoreLoggerFactory { get; } = LoggerFactory.Create(builder => builder.AddAzureWebAppDiagnostics());

        /// <summary>
        /// The Microsoft.Extensions.LoggerFactory implementation of CreateLogger(string category) uses lock(_sync) before looking in its dictionary.
        /// We'll use our own ConcurrentDictionary for performance, since we lookup the category on every log write.
        /// </summary>
        readonly ConcurrentDictionary<string, Microsoft.Extensions.Logging.ILogger> loggerCategories = new ConcurrentDictionary<string, Microsoft.Extensions.Logging.ILogger>();

        readonly ITextFormatter textFormatter;

        public AzureAppSink(ITextFormatter textFormatter)
        {
            this.textFormatter = textFormatter ?? throw new ArgumentNullException(nameof(textFormatter));
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            var sr = new StringWriter();
            textFormatter.Format(logEvent, sr);
            var text = sr.ToString().Trim();

            var category = logEvent.Properties.TryGetValue("SourceContext", out var value) ? value.ToString() : "";
            var logger = loggerCategories.GetOrAdd(category, s => CoreLoggerFactory.CreateLogger(s));

            switch (logEvent.Level)
            {
                case LogEventLevel.Fatal:
                    logger.LogCritical(text);
                    break;
                case LogEventLevel.Error:
                    logger.LogError(text);
                    break;
                case LogEventLevel.Warning:
                    logger.LogWarning(text);
                    break;
                case LogEventLevel.Information:
                    logger.LogInformation(text);
                    break;
                case LogEventLevel.Debug:
                    logger.LogDebug(text);
                    break;
                case LogEventLevel.Verbose:
                    logger.LogTrace(text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("logeventlevel");
            }
        }
    }
}
