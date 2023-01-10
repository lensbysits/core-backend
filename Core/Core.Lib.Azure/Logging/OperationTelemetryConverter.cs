using Microsoft.ApplicationInsights.Channel;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using System;

namespace Lens.Core.Lib.Azure.Logging;

public class OperationTelemetryConverter : TraceTelemetryConverter
{
    const string OperationId = "Operation Id";
    const string ParentId = "Parent Id";

    public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
    {
        foreach (var telemetry in base.Convert(logEvent, formatProvider))
        {
            if (TryGetScalarProperty(logEvent, OperationId, out var operationId) && operationId != null)
                telemetry.Context.Operation.Id = operationId.ToString();

            if (TryGetScalarProperty(logEvent, ParentId, out var parentId) && parentId != null)
                telemetry.Context.Operation.ParentId = parentId.ToString();

            yield return telemetry;
        }
    }

    private static bool TryGetScalarProperty(LogEvent logEvent, string propertyName, out object? value)
    {
        var hasScalarValue =
            logEvent.Properties.TryGetValue(propertyName, out var someValue) &&
            (someValue is ScalarValue);

        value = hasScalarValue ? (someValue as ScalarValue)?.Value : default;

        return hasScalarValue;
    }
}
