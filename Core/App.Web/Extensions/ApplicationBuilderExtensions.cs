using Lens.Core.Lib.Services;
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

        public static IApplicationBuilder AddSwaggerUI(this IApplicationBuilder appBuilder, IConfiguration configuration)
        {
            var oAuthClientSettings = configuration.GetSection(nameof(OAuthClientSettings)).Get<OAuthClientSettings>();

            appBuilder.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("swagger/v1/swagger.json", "API V1");
                options.RoutePrefix = string.Empty;

                options.OAuthClientId(oAuthClientSettings["Swagger"].ClientId);
                options.OAuthClientSecret(oAuthClientSettings["Swagger"].ClientSecret);
                options.OAuthUsePkce();
            });
            return appBuilder;
        }
    }
}
