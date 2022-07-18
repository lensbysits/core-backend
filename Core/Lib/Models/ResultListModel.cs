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
        }
        public ResultListModel(TValue[] value)
        {
            Value = value;
        }

        public int Size { get => Value.Count(); }
    }
}
