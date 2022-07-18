using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Azure.Logging
{
    public class OperationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;

            if (activity is null) return;

            logEvent.AddPropertyIfAbsent(new LogEventProperty("Operation Id", new ScalarValue(activity.TraceId)));
            //logEvent.AddPropertyIfAbsent(new LogEventProperty("Operation Id", new ScalarValue(activity.Id)));
            logEvent.AddPropertyIfAbsent(new LogEventProperty("Parent Id", new ScalarValue(activity.ParentId)));

            if (activity.Parent != null)
            {
                // Like internal method in AI W3CUtilities.FormatTelemetryId
                var parentId = $"|{activity.TraceId}.{activity.Parent.SpanId}.";
                logEvent.AddPropertyIfAbsent(new LogEventProperty("Operation Id", new ScalarValue(parentId)));
            }
        }
    }
}
