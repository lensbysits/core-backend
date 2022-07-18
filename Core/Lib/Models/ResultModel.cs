using System.Collections.Generic;

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
}
public class ResultModel<TValue> : IResultModel<TValue>
{
    public TValue Value { get; set; }
}
