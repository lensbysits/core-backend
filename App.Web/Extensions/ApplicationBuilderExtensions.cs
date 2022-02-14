using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Lens.Core.App.Web
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<ErrorHandlingMiddleware>();
            return appBuilder;
        }

        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder appBuilder, IConfiguration configuration)
        {
            var swaggerSettings = configuration.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

            appBuilder.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("swagger/v1/swagger.json", swaggerSettings?.AppName ?? "API V1");
                options.RoutePrefix = string.Empty;

                options.OAuthClientId(swaggerSettings.ClientId);
                options.OAuthClientSecret(swaggerSettings.ClientSecret);
                options.OAuthUsePkce();
            });
            return appBuilder;
        }
    }
}
