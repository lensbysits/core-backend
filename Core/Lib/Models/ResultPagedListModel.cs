using System.Collections.Generic;
using System.Linq;

namespace Lens.Core.Lib.Models
{
    public class ResultPagedListModel<TValue> : ResultListModel<TValue>
    {
        public ResultPagedListModel()
        {
        }

        public ResultPagedListModel(IEnumerable<TValue> value) : base(value)
        {
        }
        public ResultPagedListModel(TValue[] value) : base(value)
        {
        }

        /// <summary>
        /// The total number of items that could be retrieved.
        /// </summary>
        public int? TotalSize { get; set; }
        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// The zero-based index of the current page where 0 returns the first {PageSize} items of the {TotalSize} items.
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Return the current page-number (where the first page is number 1).
        /// </summary>
        public int PageNumber => PageIndex + 1;
        /// <summary>
        /// The name of the property the backend should sort on.
        /// </summary>
        public string? SortingProperty { get; set; }
        /// <summary>
        /// The sorting order. Valid values are SortingDirections.Ascending and SortingDirections.Descending
        /// </summary>
        public string? SortingDirection { get; set; }

        /// <summary>
        /// The model that was used when creating this result. 
        /// When set, its values will be used to populate:
        /// PageSize, PageIndex, SortingProperty, SortingDirection.
        /// </summary>
        private QueryModel originalQueryModel;
        public QueryModel OriginalQueryModel 
        {
            get => originalQueryModel;
            set
            {
                originalQueryModel = value;

                var orderBy = value.OrderBy?.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                PageSize = value.Limit;
                PageIndex = value.Offset / value.Limit;
                if (orderBy?.Length > 0)
                {
                    SortingProperty = orderBy?[0];
                    SortingDirection = orderBy?.Length > 1 ? orderBy[1] : SortingDirections.Ascending;
                }
            }
        }
    }
}