namespace Lens.Core.Lib.Models
{
    public interface IResultModel { }

    public interface IResultModel<TValue> : IResultModel
    {
        string ValueType { get; }
        TValue Value { get; set; }
    }
}