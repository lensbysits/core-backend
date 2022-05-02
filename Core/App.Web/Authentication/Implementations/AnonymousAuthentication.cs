using Lens.Core.Lib.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Linq;

namespace Lens.Core.App.Web.Authentication
{
    /// <summary>
    /// Used when there isn't configured any authentication configuration.
    /// </summary>
    /// <seealso cref="Lens.Core.App.Web.Authentication.IAuthententicationMethod" />
    internal sealed class AnonymousAuthentication : IAuthententicationMethod
    {
        public void ApplyMvcFilters(FilterCollection filters)
        {
            foreach (var filter in filters.OrEmpty().ToArray())
            {
                if (filter is AuthorizeFilter)
                {
                    filters.Remove(filter);
                }
            }
        }

        public void Configure(IServiceCollection services, Action<AuthorizationOptions> authorizationOptions)
        {
            services.AddHttpContextAccessor();
        }

        public void ConfigureSwaggerAuth(SwaggerGenOptions options, SwaggerSettings swaggerSettings) { }
        public void UseMiddleware(IApplicationBuilder applicationBuilder) { }
        public void UseSwaggerUI(SwaggerUIOptions options, SwaggerSettings swaggerSettings) { }
    }
}
