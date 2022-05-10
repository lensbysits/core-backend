﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace Lens.Core.Lib.Models
{
    public class QueryModel
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 50;
        /// <summary>
        /// The parameter doesn't take into account the set or default offset or limit
        /// </summary>
        public bool NoLimit { get;set; } = false;
        
        // Filter by fields:
        public string Tag { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedSince { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedSince { get; set; }

        // Search by fields:
        public string SearchTerm { get; set; }

        // Sort by fields:
        public string OrderBy { get; set; }

        public virtual string QueryString
        {
            get
            {
                List<string> queryStringSegments = new();
                    
                queryStringSegments.Add($"{nameof(Offset)}={Offset}");
                queryStringSegments.Add($"{nameof(Limit)}={Limit}");
                queryStringSegments.Add($"{nameof(NoLimit)}={NoLimit}");

                // Filter by
                if (!string.IsNullOrWhiteSpace(Tag))
                {
                    queryStringSegments.Add($"{nameof(Tag)}={UrlEncoder.Default.Encode(Tag)}");
                }

                if (!string.IsNullOrWhiteSpace(CreatedBy))
                {
                    queryStringSegments.Add($"{nameof(CreatedBy)}={CreatedBy}");
                }

                if (CreatedSince.HasValue)
                {
                    queryStringSegments.Add($"{nameof(CreatedSince)}={CreatedSince}");
                }

                if (!string.IsNullOrWhiteSpace(UpdatedBy))
                {
                    queryStringSegments.Add($"{nameof(UpdatedBy)}={UpdatedBy}");
                }

                if (UpdatedSince.HasValue)
                {
                    queryStringSegments.Add($"{nameof(UpdatedSince)}={UpdatedSince}");
                }

                // Search by
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    queryStringSegments.Add($"{nameof(SearchTerm)}={UrlEncoder.Default.Encode(SearchTerm)}");
                }

                // Sort by
                if (!string.IsNullOrWhiteSpace(OrderBy))
                {
                    queryStringSegments.Add($"{nameof(OrderBy)}={OrderBy}");
                }

                if (!queryStringSegments.Any()) return string.Empty;

                return $"?{string.Join('&', queryStringSegments)}";
            }
        }
    }
}
