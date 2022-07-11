using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace Lens.Core.App.Web.Authentication;

public abstract class AuthSettings
{
    public abstract string AuthenticationType { get; }

    public List<Policy> Policies { get; set; }
}

public class Policy
{
    public static AuthorizationPolicy DefaultPolicy => new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
    public AuthorizationPolicyBuilder DefaultPolicyBuilder => new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser();

    public string Name { get; set; }
    public string[] Roles { get; set; }
    public Dictionary<string, string> Claims { get; set; }

    private AuthorizationPolicy policyInstance;
    public AuthorizationPolicy PolicyInstance
    {
        get
        {
            if (policyInstance != null)
                return policyInstance;

            var builder = DefaultPolicyBuilder;

            if (Roles?.Any() ?? false)
                builder.RequireRole(Roles);

            if (Claims?.Any() ?? false)
            {
                foreach (var claim in Claims)
                {
                    if (string.IsNullOrEmpty(claim.Value))
                    {
                        builder.RequireClaim(claim.Key);
                    }
                    else
                    {
                        builder.RequireClaim(claim.Key, claim.Value.Split(','));
                    }
                }
            }

            return (policyInstance = builder.Build());
        }
    }
}