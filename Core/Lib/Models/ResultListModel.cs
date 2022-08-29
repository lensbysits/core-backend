namespace Lens.Core.Lib.Models;

public class ResultListModel<TValue> : ResultModel<IEnumerable<TValue>>
{
    public ResultListModel()
    {
        ValueType = ResultModelValueType.list;
    }

    public ResultListModel(IEnumerable<TValue> value) : this()
    {
        Value = value;
        Size = value.Count();
        ValueSize = value.Count();
    }
    public ResultListModel(TValue[] value) : this()
    {
        Value = value;
        Size = value.Length;
        ValueSize = value.Length;
    }

    [Obsolete("Please use the ValueSize or the ResultPagedListModel<TValue> to be more specific on the intent of the property.")]
    public int Size { get; set; }

    /// <summary>
    /// Returns the number of items in the value.
    /// </summary>
    public int ValueSize { get; }

}
