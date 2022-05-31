using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace Lens.Core.App.Web.Authentication
{
    internal interface IAuthententicationMethod
    {
        void Configure(IServiceCollection services, Action<AuthorizationOptions> authorizationOptions);
        void ConfigureSwaggerAuth(SwaggerGenOptions options, SwaggerSettings swaggerSettings);
        void UseMiddleware(IApplicationBuilder applicationBuilder);
        void UseSwaggerUI(SwaggerUIOptions options, SwaggerSettings swaggerSettings);
        void ApplyMvcFilters(FilterCollection filters);
    }
}