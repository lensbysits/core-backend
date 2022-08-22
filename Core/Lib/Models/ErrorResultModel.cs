using Lens.Core.Lib.Models;

namespace Lens.Core.Lib.Models;

public class ErrorResultModel : ResultModel<object?>
{
    public ErrorResultModel()
    {
        ValueType = ResultModelValueType.empty;
    }

    public string? Message { get; set; }

    public string ErrorType { get; set; } = "Undefined";

    public string? ErrorDetails { get; set; }

    public string? Stacktrace { get; set; }

    public string? CorrelationId { get; set; }

    public Dictionary<string, object>? Data { get; set; }
}
