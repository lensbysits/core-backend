namespace CoreLib.Models
{
    public class ApiResponse<T>
    {
        public ApiResponse()
        {
        }

        public ApiResponse(T value, int totalRecordCount, QueryModel queryModel = null)
        {
            Value = value;
            TotalRecordCount = totalRecordCount;
            QueryModel = queryModel;
        }

        public T Value { get; set; }
        public int TotalRecordCount { get; set; }
        public QueryModel QueryModel { get; set; }
    }

    public class ApiResponse
    {
        public static ApiResponse<TValue> From<TValue>(TValue value, int totalRecordCount, QueryModel queryModel = null)
        {
            return new ApiResponse<TValue>(value, totalRecordCount, queryModel);
        }
    }
}
