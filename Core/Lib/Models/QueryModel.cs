using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace Lens.Core.Lib.Models
{
    public class QueryModel
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 50;
        public bool NoLimit { get;set; } = false;
        
        // Filter by
        public string Tag { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedSince { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedSince { get; set; }

        // Search by
        public string SearchTerm { get; set; }

        // Sort by
        public string OrderBy { get; set; }

        public string QueryString
        {
            get
            {
                List<string> queryStringSegments = new();
                    
                queryStringSegments.Add($"offset={Offset}");
                queryStringSegments.Add($"limit={Limit}");
                queryStringSegments.Add($"noLimit={NoLimit}");

                // Filter by
                if (!string.IsNullOrWhiteSpace(Tag))
                {
                    queryStringSegments.Add($"tag={UrlEncoder.Default.Encode(Tag)}");
                }

                if (!string.IsNullOrWhiteSpace(CreatedBy))
                {
                    queryStringSegments.Add($"createdBy={CreatedBy}");
                }

                if (CreatedSince.HasValue)
                {
                    queryStringSegments.Add($"updatedSince={CreatedSince}");
                }

                if (!string.IsNullOrWhiteSpace(UpdatedBy))
                {
                    queryStringSegments.Add($"updatedBy={UpdatedBy}");
                }

                if (UpdatedSince.HasValue)
                {
                    queryStringSegments.Add($"updatedSince={UpdatedSince}");
                }

                // Search by
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    queryStringSegments.Add($"searchTerm={UrlEncoder.Default.Encode(SearchTerm)}");
                }

                // Sort by
                if (!string.IsNullOrWhiteSpace(OrderBy))
                {
                    queryStringSegments.Add($"orderBy={OrderBy}");
                }

                if (!queryStringSegments.Any()) return string.Empty;

                return $"?{string.Join('&', queryStringSegments)}";
            }
        }
    }
}
