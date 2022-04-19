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
    /// <summary>
    /// Used when there isn't configured any authentication configuration.
    /// </summary>
    /// <seealso cref="Lens.Core.App.Web.Authentication.IAuthententicationMethod" />
    internal sealed class AnonymousAuthentication : IAuthententicationMethod
    {
        public void ApplyMvcFilters(FilterCollection filters) { }
        public void Configure(IServiceCollection services, Action<AuthorizationOptions> authorizationOptions, Action<JwtBearerOptions> jwtBearerOptions)
        {
            services.AddHttpContextAccessor();
        }

        public void ConfigureSwaggerAuth(SwaggerGenOptions options, SwaggerSettings swaggerSettings) { }
        public void UseMiddleware(IApplicationBuilder applicationBuilder) { }
        public void UseSwaggerUI(SwaggerUIOptions options, SwaggerSettings swaggerSettings) { }
    }
}
