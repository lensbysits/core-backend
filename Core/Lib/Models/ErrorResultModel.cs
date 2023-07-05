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

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("errorType")]
    public string? ErrorType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("errorDetails")]
    public string? ErrorDetails { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("stacktrace")]
    public string? Stacktrace { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("data")]
    public IDictionary? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("dataDetails")]
    public Dictionary<string, object>? DataDetails { get; set; }
}
