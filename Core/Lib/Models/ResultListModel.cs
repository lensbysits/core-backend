using System.Collections.Generic;

namespace Lens.Core.Lib.Models
{
    public class ResultListModel<TResult>
    {
        public IEnumerable<TResult> Value { get; set; }
        public int Size { get; set; }
    }
}
