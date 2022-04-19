using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace Lens.Core.App.Web.Authentication
{
    internal abstract class AuthenticationBase : IAuthententicationMethod
    {
        public AuthenticationBase(AuthSettings authSettings)
        {
            AuthSettings = authSettings ?? throw new ArgumentNullException(nameof(authSettings));
        }

        protected AuthSettings AuthSettings { get; }

        public virtual void UseMiddleware(IApplicationBuilder applicationBuilder) { }
        
        public virtual void ApplyMvcFilters(FilterCollection filters) { }

        public virtual void Configure(IServiceCollection services, Action<AuthorizationOptions> authorizationOptions, Action<JwtBearerOptions> jwtBearerOptions) { }

        public virtual void ConfigureSwaggerAuth(SwaggerGenOptions options, SwaggerSettings swaggerSettings) { }

        public virtual void UseSwaggerUI(SwaggerUIOptions options, SwaggerSettings swaggerSettings) { }

    }
}
