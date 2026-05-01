using Lens.Core.App.Web.Services;
using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Text;

namespace Lens.Core.App.Web.Authentication;

internal class AzureAuthentication<T> : OAuth2Authentication<T> where T : AzureAuthSettings
{
    private const string ScopePolicyName = "ApiScopePolicy";
    private const string RolePolicyName = "ApiRolePolicy";
    private const string ScopeOrRolePolicyName = "ApiScopeOrRolePolicy";
    private static readonly string[] ScopeClaimTypes = { ClaimConstants.Scope, ClaimConstants.Scp, "scp" };
    private static readonly string[] RoleClaimTypes = { ClaimConstants.Role, ClaimConstants.Roles, ClaimTypes.Role, "roles", "role", "Role Name" };
    private readonly IConfiguration configuration;

    public AzureAuthentication(T authSettings, IConfiguration configuration) : base(authSettings)
    {
        this.configuration = configuration;
    }

    public override void ApplyMvcFilters(FilterCollection filters)
    {
        base.ApplyMvcFilters(filters);

        if (this.AuthSettings.RequiredScopes.Any() || this.AuthSettings.RequiredAppRoles.Any())
        {
            filters.Add(new AuthorizeFilter(ScopeOrRolePolicyName));
        }
    }

    public override void Configure(
        IServiceCollection services,
        Action<AuthorizationOptions>? authorizationOptions)
    {

        services.AddMicrosoftIdentityWebApiAuthentication(this.configuration, "AuthSettings");

        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = "roles";

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
                var allowedIssuers = new List<string>();

                foreach (var issuer in this.AuthSettings.AllowedIssuers)
                {
                    allowedIssuers.Add(issuer);

                    if (Guid.TryParse(issuer, out Guid issuerGuid))
                    {
                        var azureUrl = $"https://sts.windows.net/{issuerGuid}/";

                        allowedIssuers.Add(azureUrl);
                    }
                }

                // we need to override the default issuer validation in order to restrict access only for pre-configured allowed issuers
                // otherwise all Azure tenants are considered as valid issuer
                options.TokenValidationParameters.IssuerValidator = null;
                options.TokenValidationParameters.ValidIssuers = allowedIssuers;
                options.TokenValidationParameters.ValidateIssuer = true;
            }

            base.RegisterAuthenticationInterceptorEventHandlers(options);

        });

        services.AddAuthorization(
            options =>
            {
                options.AddPolicy(ScopeOrRolePolicyName, ScopeOrRolePolicy(Serilog.Log.Logger.ForContext(this.GetType())));
                options.FallbackPolicy = DefaultPolicy;

                authorizationOptions?.Invoke(options);
            });

        services.AddScoped<IUserContext, UserContext>();
    }

    /// <summary>
    /// Validates scope and app role claims exists and check against the configured required scope(s) and/or role(s).
    /// Every request must contain a scope or role claim, otherwise a 401 is returned.
    /// </summary>
    private Action<AuthorizationPolicyBuilder> ScopeOrRolePolicy(Serilog.ILogger? logger = null)
    {
        return policy => policy.RequireAssertion(context =>
                {
                    var incomingScopes = GetClaimValues(context.User, ScopeClaimTypes).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                    var incomingRoles = GetClaimValues(context.User, RoleClaimTypes).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                    var isAppOnlyToken = IsAppOnlyToken(context.User);

                    var logStr = $"Authz:ScopePolicy: User object id: {context.User?.GetObjectId()} of tenant: {context.User?.GetTenantId()} Found scopes: {string.Join(' ', incomingScopes)} Found roles: {string.Join(' ', incomingRoles)} App-only token: {isAppOnlyToken} ";

                    if (this.AuthSettings.RequiredScopes.Any() && incomingScopes.Any())
                    {
                        var accessAllowed = this.AuthSettings.RequiredScopes.Any(
                            s => incomingScopes.Contains(s, StringComparer.OrdinalIgnoreCase));

                        if (logger != null)
                        {
                            logStr += (accessAllowed ? "Access Allowed by scope" : "Access NOT Allowed by scope");
                            logger.Information(logStr);
                        }

                        if (accessAllowed)
                        {
                            return true;
                        }
                    }

                    if (this.AuthSettings.RequiredAppRoles.Any() && incomingRoles.Any())
                    {
                        var accessAllowed = (!this.AuthSettings.RolesForApplicationsOnly || isAppOnlyToken)
                            && this.AuthSettings.RequiredAppRoles.Any(
                                s => incomingRoles.Contains(s, StringComparer.OrdinalIgnoreCase));

                        if (logger != null)
                        {
                            logStr += (accessAllowed ? "Access Allowed by role" : "Access NOT Allowed by role");
                            logger.Information(logStr);
                        }

                        if (accessAllowed)
                        {
                            return true;
                        }
                    }

                    return false;
                });
    }

    private static IEnumerable<string> GetClaimValues(ClaimsPrincipal? user, IEnumerable<string> claimTypes)
    {
        if (user == null)
        {
            return Enumerable.Empty<string>();
        }

        var claimTypeSet = claimTypes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return user.Claims
            .Where(claim => claimTypeSet.Contains(claim.Type))
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(value => !string.IsNullOrWhiteSpace(value));
    }

    private static bool IsAppOnlyToken(ClaimsPrincipal? user)
    {
        if (user == null)
        {
            return false;
        }

        if (user.HasClaim(claim =>
            string.Equals(claim.Type, "idtyp", StringComparison.OrdinalIgnoreCase)
            && string.Equals(claim.Value, "app", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        var hasDelegatedScope = GetClaimValues(user, ScopeClaimTypes).Any();
        var hasClientAppClaim = user.HasClaim(claim =>
            string.Equals(claim.Type, "azp", StringComparison.OrdinalIgnoreCase)
            || string.Equals(claim.Type, "appid", StringComparison.OrdinalIgnoreCase)
            || string.Equals(claim.Type, "client_id", StringComparison.OrdinalIgnoreCase));

        return hasClientAppClaim && !hasDelegatedScope;
    }
}
