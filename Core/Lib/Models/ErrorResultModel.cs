using System.Collections;
using System.Text.Json.Serialization;

namespace Lens.Core.Lib.Models;

public class ErrorResultModel : ResultModel<object?>
{
    public ErrorResultModel()
    {
        ValueType = ResultModelValueType.empty;
    }

    public ErrorResultModel(string correlationId, string? errorCode) : this()
    {
        this.CorrelationId = correlationId;
        this.ErrorCode = errorCode;
    }

    public string? Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorType { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorDetails { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Stacktrace { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CorrelationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IDictionary? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]    
    public Dictionary<string, object>? DataDetails { get; set; }
}
