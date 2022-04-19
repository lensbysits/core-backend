using Lens.Core.App.Web.Services;
using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Lens.Core.App.Web.Authentication
{
    internal class OAuth2Authentication : AuthenticationBase
    {
        protected static AuthorizationPolicy DefaultPolicy
        {
            get =>
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
        }

        public OAuth2Authentication(AuthSettings authSettings) : base(authSettings)
        {
        }

        public override void Configure(
            IServiceCollection services,
            Action<AuthorizationOptions> authorizationOptions,
            Action<JwtBearerOptions> jwtBearerOptions)
        {
            services
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Audience = this.AuthSettings.Audience;
                        options.Authority = this.AuthSettings.Authority;

                        if (!string.IsNullOrEmpty(this.AuthSettings.MetadataAddress))
                            options.MetadataAddress = this.AuthSettings.MetadataAddress;

                        options.RequireHttpsMetadata = this.AuthSettings.RequireHttps;
                        options.TokenValidationParameters.ValidateAudience = this.AuthSettings.ValidateAudience;
                        options.TokenValidationParameters.NameClaimType = "name";

                        jwtBearerOptions?.Invoke(options);
                    });

            authorizationOptions ??= options => { options.DefaultPolicy = DefaultPolicy; };
            services.AddAuthorization(authorizationOptions);

            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
        }

        public override void ConfigureSwaggerAuth(SwaggerGenOptions options, SwaggerSettings swaggerSettings)
        {
            base.ConfigureSwaggerAuth(options, swaggerSettings);

            //https://www.c-sharpcorner.com/article/enable-oauth-2-authorization-using-azure-ad-and-swagger-in-net-5-0/
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{swaggerSettings.Authority}connect/authorize"),
                        TokenUrl = new Uri($"{swaggerSettings.Authority}connect/token"),
                        Scopes = new Dictionary<string, string>
                                {
                                    {swaggerSettings.Scope, swaggerSettings.ScopeName}
                                }
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        }

        public override void UseSwaggerUI(SwaggerUIOptions options, SwaggerSettings swaggerSettings)
        {
            base.UseSwaggerUI(options, swaggerSettings);

            //https://lurumad.github.io/swagger-ui-with-pkce-using-swashbuckle-asp-net-core
            options.OAuthClientId(swaggerSettings.ClientId);
            options.OAuthClientSecret(swaggerSettings.ClientSecret);
            options.OAuthUsePkce();
        }
    }
}
