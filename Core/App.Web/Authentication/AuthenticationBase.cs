using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace Lens.Core.App.Web.Authentication;

internal abstract class AuthenticationBase<T> : IAuthenticationMethod where T : AuthSettings
{
    public AuthenticationBase(T authSettings)
    {
        AuthSettings = authSettings ?? throw new ArgumentNullException(nameof(authSettings));
    }

    protected T AuthSettings { get; }

    public virtual void UseMiddleware(IApplicationBuilder applicationBuilder) {
        // default use authentication middleware (otherwise override it)
        applicationBuilder.UseAuthentication();
    }
    
    public virtual void ApplyMvcFilters(FilterCollection filters) { }

    public virtual void Configure(IServiceCollection services, Action<AuthorizationOptions> authorizationOptions) { }

    public virtual void ConfigureSwaggerAuth(SwaggerGenOptions options, SwaggerSettings swaggerSettings) { }

    public virtual void UseSwaggerUI(SwaggerUIOptions options, SwaggerSettings swaggerSettings) { }

}
