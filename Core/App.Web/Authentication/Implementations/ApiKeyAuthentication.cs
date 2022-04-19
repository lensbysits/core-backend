using Lens.Core.App.Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Lens.Core.App.Web.Authentication
{
    internal class ApiKeyAuthentication : AuthenticationBase
    {
        public ApiKeyAuthentication(AuthSettings authSettings) : base(authSettings)
        {
        }

        public override void UseMiddleware(IApplicationBuilder applicationBuilder)
        {
            base.UseMiddleware(applicationBuilder);

            applicationBuilder.UseMiddleware<ApiKeyMiddleware>();
        }

        public override void ConfigureSwaggerAuth(SwaggerGenOptions options, SwaggerSettings swaggerSettings)
        {
            base.ConfigureSwaggerAuth(options, swaggerSettings);

            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = this.AuthSettings.ApiKeyHeader,
                Description = "API Key Authentication",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
}
