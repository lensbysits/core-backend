using Lens.Core.App.Web.Services;
using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lens.Core.App.Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration,
            Action<CorsOptions> corsConfigureOptions = null)
        {
            services.AddCors(corsOptions =>
            {
                corsOptions.AddDefaultPolicy(defaultPolicyBuilder =>
                {
                    string[] origins = GetCorsOrigins(configuration);

                    defaultPolicyBuilder
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();

                    if (!origins.Contains("*"))
                    {
                        defaultPolicyBuilder.AllowCredentials();
                    };
                });

                corsConfigureOptions?.Invoke(corsOptions);
            });

            return services;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration
                , Action<AuthorizationOptions> authorizationOptions = null
                , Action<JwtBearerOptions> jwtBearerOptions = null)
        {
            if (configuration["ASPNETCORE_ENVIRONMENT"] == Microsoft.Extensions.Hosting.Environments.Development)
            {
                // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/PII
                IdentityModelEventSource.ShowPII = true;
            }

            var authSettings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    if (authSettings != null)
                    {
                        options.Audience = authSettings.Audience;
                        options.Authority = authSettings.Authority;

                        if (!string.IsNullOrEmpty(authSettings.MetadataAddress))
                            options.MetadataAddress = authSettings.MetadataAddress;

                        options.RequireHttpsMetadata = authSettings.RequireHttps;
                        options.TokenValidationParameters.ValidateAudience = authSettings.ValidateAudience;
                        options.TokenValidationParameters.NameClaimType = "name";
                    }

                    jwtBearerOptions?.Invoke(options);
                });

            authorizationOptions ??= options => { options.DefaultPolicy = DefaultPolicy; };
            services.AddAuthorization(authorizationOptions);

            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }


        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerSettings = configuration.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = swaggerSettings.AppName ?? "Protected API", 
                    Version = "v1" 
                });

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
            });

            return services;
        }

        private static AuthorizationPolicy DefaultPolicy
        {
            get =>
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
        }

        private static string[] GetCorsOrigins(IConfiguration configuration)
        {
            var corsSettings = configuration.GetSection(nameof(CorsSettings)).Get<CorsSettings>();
            var origins = corsSettings?.Origins?
                .Trim()
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();
            if (origins == null || origins.Length == 0)
            {
                origins = new[] { "*" };
            }

            return origins;
        }
    }
}
