using Lens.Core.App.Web.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Lens.Core.App.Web.Middleware;

/// <summary>
/// A simple API key middleware implementation using the header X-Api-Key
/// </summary>
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeyAuthSettings authSettings;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        authSettings = configuration.GetSection(nameof(AuthSettings)).Get<ApiKeyAuthSettings>();
    }

    public async Task Invoke(HttpContext context, IConfiguration configuration)
    {
        //var authSettings = configuration.GetSection(nameof(AuthSettings)).Get<ApiKeyAuthSettings>();

        if (!string.IsNullOrEmpty(authSettings.AuthenticationType) && authSettings.AuthenticationType.Equals("apikey", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrEmpty(authSettings.ApiKey) || string.IsNullOrEmpty(authSettings.ApiKeyHeader))
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("API Key authentication is used, but not being configured correctly");
                return;
            }

            if (!context.Request.Headers.ContainsKey(authSettings.ApiKeyHeader))
            {
                context.Response.StatusCode = 400; //Bad Request                
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            if (!authSettings.ApiKey.Equals(context.Request.Headers[authSettings.ApiKeyHeader]))
            {
                context.Response.StatusCode = 401; //UnAuthorized
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }
        }
        await _next.Invoke(context);
    }
}
