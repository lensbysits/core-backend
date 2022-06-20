using Lens.Core.App.Web.Services;
using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                options.TokenValidationParameters.NameClaimType = "name";

                if (this.AuthSettings.IncludeConfigInBearerHeader)
                {
                    var buildHeader = new StringBuilder("Bearer");
                    if (!string.IsNullOrEmpty(this.AuthSettings.Authority))
                    {
                        buildHeader.AppendFormat(" authorization_uri=\"{0}authorize\"", this.AuthSettings.Authority);
                    }

                    if (!string.IsNullOrEmpty(this.AuthSettings.Resource))
                    {
                        buildHeader.AppendFormat(", resource=\"{0}\"", this.AuthSettings.Resource);
                    }

                    options.Challenge = buildHeader.ToString().Trim();
                }
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
                    options.AddPolicy(ScopePolicyName, ScopePolicy(logger));
                    options.AddPolicy(RolePolicyName, RolePolicy(logger));
                    options.FallbackPolicy = DefaultPolicy;

                    authorizationOptions?.Invoke(options);
                });

            services.AddScoped<IUserContext, UserContext>();
        }
        
        private Action<AuthorizationPolicyBuilder> ScopePolicy(ILogger logger = null)
        {
            return policy => policy.RequireAssertion(
                                        context =>
                                        {
                                            
                                            var scopeClaim = context.User.FindFirst(ClaimConstants.Scope) ?? context.User.FindFirst(ClaimConstants.Scp);

                                            var logStr = $"Authz:ScopePolicy: User object id: {context.User?.GetObjectId()} Found scopes: {scopeClaim?.Value} ";

                                            if (scopeClaim == null)
                                            {
                                                /// For a confidential client, the value is 1 when a shared secret (a password) is used as a client secret (app authentication) and 
                                                /// 2 when a certificate is used as a client secret (app authentication). 
                                                /// The value 0 indicates a public client, which does not provide a client secret (user authentication)
                                                var authType = context.User.FindFirst(ClaimConstants.Acr) ?? context.User.FindFirst("appidacr");
                                                var hasClientSecret = (authType != null && (authType.Value == "1" || authType.Value == "2"));

                                                logStr += $"Has Client Secret: {hasClientSecret.ToString()} ";

                                                if (logger != null)
                                                {
                                                    logger.LogInformation(logStr);
                                                }

                                                return hasClientSecret;
                                            }
                                            
                                            var incommingScopes = scopeClaim.Value.Split(' ');
                                            var accessAllowed = this.AuthSettings.RequiredScopes.All(
                                                s => incommingScopes.Contains(s));

                                            if (logger != null)
                                            {
                                                logStr += (accessAllowed ? "Access Allowed by scope" : "Access NOT Allowed by scope");
                                                logger.LogInformation(logStr);
                                            }
                                            return accessAllowed;
                                        });
        }

        private Action<AuthorizationPolicyBuilder> RolePolicy(ILogger logger = null)
        {
            return policy => policy.RequireAssertion(
                                        context =>
                                        {
                                            var roleClaim = context.User.FindFirst(ClaimConstants.Role) ?? context.User.FindFirst(ClaimConstants.Roles);

                                            var logStr = $"Authz:RolePolicy: User object id: {context.User?.GetObjectId()} Found roles: {roleClaim?.Value} ";

                                            if (roleClaim == null)
                                            {
                                                /// For a confidential client, the value is 1 when a shared secret (a password) is used as a client secret (app authentication) and 
                                                /// 2 when a certificate is used as a client secret (app authentication). 
                                                /// The value 0 indicates a public client, which does not provide a client secret (user authentication)
                                                var authType = context.User.FindFirst(ClaimConstants.Acr) ?? context.User.FindFirst("appidacr");
                                                var isPublicClient = this.AuthSettings.RolesForApplicationsOnly && authType != null && (authType.Value == "0");

                                                logStr += $"Is Public Client: {isPublicClient.ToString()} ";

                                                if (logger != null)
                                                {
                                                    logger.LogInformation(logStr);
                                                }

                                                return isPublicClient;
                                            }

                                            var incommingScopes = roleClaim.Value.Split(' ');
                                            var accessAllowed = this.AuthSettings.RequiredAppRoles.All(
                                                s => incommingScopes.Contains(s));

                                            if (logger != null)
                                            {
                                                logStr += (accessAllowed ? "Access Allowed by role" : "Access NOT Allowed by role");
                                                logger.LogInformation(logStr);
                                            }

                                            return accessAllowed;
                                        });
        }
    }
}
