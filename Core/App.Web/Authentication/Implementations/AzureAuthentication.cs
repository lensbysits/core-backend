using Lens.Core.App.Web.Services;
using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lens.Core.App.Web.Authentication
{
    internal class AzureAuthentication<T> : OAuth2Authentication<T> where T : AzureAuthSettings
    {
        private const string ScopePolicyName = "ApiScopePolicy";
        private const string RolePolicyName = "ApiRolePolicy";
        private readonly IConfiguration configuration;

        public AzureAuthentication(T authSettings, IConfiguration configuration) : base(authSettings)
        {
            this.configuration = configuration;
        }

        public override void ApplyMvcFilters(FilterCollection filters)
        {
            base.ApplyMvcFilters(filters);

            if (this.AuthSettings.RequiredScopes.Any())
            {
                filters.Add(new AuthorizeFilter(ScopePolicyName));
            }

            if (this.AuthSettings.RequiredAppRoles.Any())
            {
                filters.Add(new AuthorizeFilter(RolePolicyName));
            }
        }

        public override void Configure(
            IServiceCollection services,
            Action<AuthorizationOptions> authorizationOptions)
        {
            services.AddMicrosoftIdentityWebApiAuthentication(this.configuration, "AuthSettings");

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                if (this.AuthSettings.AllowedIssuers.Any())
                {
                    // we need to override the default issuer validation in order to restrict access only for pre-configured allowed issuers
                    // otherwise all Azure tenants are considered as valid issuer
                    options.TokenValidationParameters.IssuerValidator = null;
                    options.TokenValidationParameters.ValidIssuers = this.AuthSettings.AllowedIssuers;
                    options.TokenValidationParameters.ValidateIssuer = true;
                }

                base.RegisterAuthenticationInterceptorEventHandlers(options);

            });

            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy(ScopePolicyName, ScopePolicy());
                    options.AddPolicy(RolePolicyName, RolePolicy());
                    options.FallbackPolicy = DefaultPolicy;

                    authorizationOptions?.Invoke(options);
                });

            services.AddScoped<IUserContext, UserContext>();
        }
        
        private Action<AuthorizationPolicyBuilder> ScopePolicy()
        {
            return policy => policy.RequireAssertion(
                                        context =>
                                        {
                                            var scopeClaim = context.User.FindFirst(ClaimConstants.Scope) ?? context.User.FindFirst(ClaimConstants.Scp);
                                            if (scopeClaim == null)
                                            {
                                                return false;
                                            }

                                            var incommingScopes = scopeClaim.Value.Split(' ');
                                            var accessAllowed = this.AuthSettings.RequiredScopes.All(
                                                s => incommingScopes.Contains(s));
                                            return accessAllowed;
                                        });
        }

        private Action<AuthorizationPolicyBuilder> RolePolicy()
        {
            return policy => policy.RequireAssertion(
                                        context =>
                                        {
                                            var roleClaim = context.User.FindFirst(ClaimConstants.Role) ?? context.User.FindFirst(ClaimConstants.Roles);
                                            if (roleClaim == null)
                                            {
                                                return false;
                                            }

                                            var incommingScopes = roleClaim.Value.Split(' ');
                                            var accessAllowed = this.AuthSettings.RequiredAppRoles.All(
                                                s => incommingScopes.Contains(s));
                                            return accessAllowed;
                                        });
        }
    }
}
