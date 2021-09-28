using Microsoft.AspNetCore.Builder;

namespace CoreApp.Web
{
    public static class ApplicationBuilderExtentions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<ErrorHandlingMiddleware>();
            return appBuilder;
        }
    }
}
