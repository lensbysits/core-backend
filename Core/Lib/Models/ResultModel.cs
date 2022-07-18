namespace Lens.Core.Lib.Models;

public class ResultModel<TValue> : IResultModel<TValue>
{
    public TValue Value { get; set; }
}
