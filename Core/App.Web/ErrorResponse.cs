using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.App.Web
{
    public class ErrorResponse
    {

        public string Message { get; set; }

        public string ErrorType { get; set; } = "Undefined";

        public string? ErrorDetails { get; set; }

        public string? Stacktrace { get; set; }

        public string? CorrelationId { get; set; }

        public Dictionary<string, object>? Data { get; set; }
    }
}
