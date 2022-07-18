namespace Lens.Core.Lib.Models
{
    public interface IResultModel { }

    public interface IResultModel<TValue> : IResultModel
    {
        TValue Value { get; set; }
    }
}