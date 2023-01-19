﻿using System.Text.Json.Serialization;

namespace Lens.Core.Lib.Models;

public static class ResultModel
{
    public static IResultModel From<T>(T value)
    {
        return new ResultModel<T> {  Value = value };
    }

    public static IResultModel From<T>(IEnumerable<T> value)
    {
        return new ResultListModel<T>(value);
    }

    public static IResultModel From<T>(IEnumerable<T> value, QueryModel queryModel)
    {
        var result = new ResultPagedListModel<T>(value)
        {
            OriginalQueryModel = queryModel
        };
        return result;
    }
}

public class ResultModel<TValue> : IResultModel<TValue>
{
    public ResultModel()
    {
        ValueType = ResultModelValueType.@object;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("valueType")]
    public string ValueType { get; protected set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("value")]
    public TValue? Value { get; set; }

}
