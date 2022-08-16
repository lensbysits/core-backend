using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lens.Core.Lib.Extensions
{
    public static class ExceptionExtensions
    {
        public static Dictionary<string, object> GetSerializableDataDictionary(this Exception e, bool includeInnerExceptionData = false)
        {
            if (e == null || e.Data == null || e.Data.Count == 0)
            {
                return null;
            }

            Dictionary<string, object> dataResult = new Dictionary<string, object>();

            foreach (var k in e.Data.Keys)
            {
                if (k == null) // this use case has happened before
                {
                    continue;
                }

                dataResult.TryAdd(k.ToString(), e.Data[k]);
            }

            if (includeInnerExceptionData && e.InnerException != null)
            {
                var innerExceptionData = GetSerializableDataDictionary(e.InnerException, includeInnerExceptionData);

                if (innerExceptionData != null && innerExceptionData.Count > 0)
                {
                    foreach (var k in innerExceptionData.Keys)
                    {
                        if (!dataResult.ContainsKey(k.ToString()))
                        {
                            dataResult.TryAdd(k.ToString(), innerExceptionData[k]);
                        }
                    }
                }
            }

            return dataResult;
        }


        public static string GetFullExceptionData(this Exception e)
        {
            if (e == null)
            {
                return string.Empty;
            }

            StringBuilder exceptionMessage = new StringBuilder(e.Message);

            exceptionMessage.Append(" (" + e.GetType().Name + "):" + Environment.NewLine);

            if (e.Data != null && e.Data?.Count > 0)
            {
                var items = new List<string>(e.Data.Count);
                foreach (var k in e.Data.Keys)
                {
                    if (k == null)
                    {
                        continue;
                    }

                    var v = e.Data[k];

                    if (v != null && v.GetType().IsValueType)
                    {
                        items.Add(string.Concat(k.ToString(), ": ", v.ToString()));
                    }
                }

                if (items.AnyOrDefault())
                {
                    exceptionMessage.AppendLine(string.Concat("\tError Data: ", string.Join(", ", items)));
                }
            }

            exceptionMessage.AppendLine(String.Concat("\t Source: ", e.Source ?? ""));
            exceptionMessage.AppendLine(String.Concat("\t Stack Trace: ", e.StackTrace ?? ""));

            if (e.InnerException != null)
            {
                exceptionMessage.AppendLine("Inner Exception: ");


                var fullE = GetFullExceptionData(e.InnerException);

                if (!string.IsNullOrEmpty(fullE))
                {
                    exceptionMessage.AppendLine(fullE);
                }
            }

            return exceptionMessage.ToString();
        }
    }
}
