using Lens.Core.App.Web.Services;
using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Lens.Core.App.Web.Authentication;

internal class OAuth2Authentication<T> : AuthenticationBase<T> where T : OAuthSettings
{
    protected static AuthorizationPolicy DefaultPolicy
    {
        get =>
            new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
    }

    public OAuth2Authentication(T authSettings) : base(authSettings)
    {
    }

    public override void Configure(
        IServiceCollection services,
        Action<AuthorizationOptions> authorizationOptions)
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

                this.RegisterAuthenticationInterceptorEventHandlers(options);
            });

        authorizationOptions ??= options => { options.DefaultPolicy = DefaultPolicy; };

        if (this.AuthSettings.Policies?.Any() ?? false)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = Policy.DefaultPolicy;
                foreach (var policy in this.AuthSettings.Policies)
                {
                    options.AddPolicy(policy.Name, policy.PolicyInstance);
                }
            });
        }

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
    }

    public override void ConfigureSwaggerAuth(SwaggerGenOptions options, SwaggerSettings swaggerSettings)
    {
        if (swaggerSettings == null)
            return;

        //https://www.c-sharpcorner.com/article/enable-oauth-2-authorization-using-azure-ad-and-swagger-in-net-5-0/
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{swaggerSettings.Authority}authorize"),
                    TokenUrl = new Uri($"{swaggerSettings.Authority}token"),
                    Scopes = new Dictionary<string, string>
                            {
                                {swaggerSettings.Scope ?? string.Empty, swaggerSettings.ScopeName ?? string.Empty }
                            }
                }
            }
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                },
                new[] { swaggerSettings.Scope }
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

    protected void RegisterAuthenticationInterceptorEventHandlers(JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = async context => await HandleEvent(context, async interceptor => await interceptor.OnMessageReceived(context)),
            OnTokenValidated = async context => await HandleEvent(context, async interceptor => await interceptor.OnTokenValidated(context)),
            OnChallenge = async context => await HandleEvent(context, async interceptor => await interceptor.OnChallenge(context)),
            OnForbidden = async context => await HandleEvent(context, async interceptor => await interceptor.OnForbidden(context)),
            OnAuthenticationFailed = async context => await HandleEvent(context, async interceptor => await interceptor.OnAuthenticationFailed(context))
        };
    }

    private static async Task HandleEvent<TContext>(TContext context, Func<IAuthenticationInterceptor, Task> action) where TContext : BaseContext<JwtBearerOptions>
    {
        var interceptors = context.HttpContext.RequestServices.GetServices<IAuthenticationInterceptor>();
        foreach (var interceptor in interceptors)
        {
            await action(interceptor);
        }
    }
}
