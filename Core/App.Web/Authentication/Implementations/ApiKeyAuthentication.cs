using Lens.Core.App.Web.Middleware;
using Lens.Core.Lib.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lens.Core.App.Web.Authentication;

internal class ApiKeyAuthentication<T> : AuthenticationBase<T> where T : ApiKeyAuthSettings
{
    public ApiKeyAuthentication(T authSettings) : base(authSettings)
    {
    }

    public override void UseMiddleware(IApplicationBuilder applicationBuilder)
    {
        // don't use the base one
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


        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("ApiKey", document)] = new List<string>()
        });
    }

    public override void ApplyMvcFilters(FilterCollection filters)
    {
        base.ApplyMvcFilters(filters);


        foreach (var filter in filters.OrEmpty().ToArray())
        {
            if (filter is AuthorizeFilter)
            {
                filters.Remove(filter);
            }
        }
    }
}
