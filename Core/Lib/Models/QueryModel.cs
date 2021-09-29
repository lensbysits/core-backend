using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace Lens.Core.Lib.Models
{
    public class QueryModel
    {
        public int? Offset { get; set; } = 0;
        public int? Limit { get; set; } = 50;

        public string Q { get; set; }
        public string SortField { get; set; }
        public SortOrder? SortOrder { get; set; } = Models.SortOrder.asc;

        public DateTime? UpdatedSince { get; set; }

        public string QueryString
        {
            get
            {
                var queryStringSegments = new List<string>();
                if (Offset.HasValue)
                    queryStringSegments.Add($"offset={Offset}");

                if (Limit.HasValue)
                    queryStringSegments.Add($"limit={Limit}");

                if (!string.IsNullOrEmpty(Q))
                    queryStringSegments.Add($"q={UrlEncoder.Default.Encode(Q)}");

                if (!string.IsNullOrEmpty(SortField))
                    queryStringSegments.Add($"sortField={SortField}");

                if (SortOrder.HasValue)
                    queryStringSegments.Add($"sortOrder={SortOrder}");

                if (UpdatedSince.HasValue)
                    queryStringSegments.Add($"updatedSince={UpdatedSince}");

                if (!queryStringSegments.Any())
                    return string.Empty;

                return $"?{string.Join('&', queryStringSegments)}";
            }
        }
    }

    public enum SortOrder
    {
        asc,
        desc
    }
}
