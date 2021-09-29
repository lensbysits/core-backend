using Microsoft.AspNetCore.Builder;

namespace Lens.Core.App.Web
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<ErrorHandlingMiddleware>();
            return appBuilder;
        }
    }
}
