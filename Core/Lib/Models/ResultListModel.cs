using System.Collections.Generic;
using System.Linq;

namespace Lens.Core.Lib.Models
{
    public class ResultListModel<TValue> : ResultModel<IEnumerable<TValue>>
    {
        public ResultListModel()
        {
        }

        public ResultListModel(IEnumerable<TValue> value)
        {
            Value = value;
            Size = value.Count();
        }
        public ResultListModel(TValue[] value)
        {
            Value = value;
            Size = value.Length;
        }

        public int Size { get; set; }
    }
}
