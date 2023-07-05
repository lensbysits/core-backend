using Microsoft.Extensions.Configuration;

namespace Lens.Core.App.Web.Authentication;

internal static class AuthenticationFactory
{
    private static readonly Dictionary<string, IAuthenticationMethod> methods = new Dictionary<string, IAuthenticationMethod>();
    public static IAuthenticationMethod GetAuthenticationMethod(IConfiguration configuration)
    {
        var authSection = configuration.GetSection(nameof(AuthSettings));
        return GetAuthenticationMethodByType(authSection, configuration);
    }

    private static IAuthenticationMethod GetAuthenticationMethodByType(IConfigurationSection authSection, IConfiguration configuration)
    {
        if (!authSection?.Exists() ?? false)
        {
            return InitializeAuthenticationMethod(AuthenticationMethod.Anonymous, () => new AnonymousAuthentication());
        }

        var type = authSection.GetValue<string>("AuthenticationType").ToLowerInvariant();


        switch (type)
        {
            case AuthenticationMethod.OAuth2:
                return InitializeAuthenticationMethod(type,
                    () => new OAuth2Authentication<OAuthSettings>(authSection.Get<OAuthSettings>()));

            case AuthenticationMethod.ApiKey:
                return InitializeAuthenticationMethod(type,
                    () => new ApiKeyAuthentication<ApiKeyAuthSettings>(authSection.Get<ApiKeyAuthSettings>()));

            case AuthenticationMethod.AzureAd:
                return InitializeAuthenticationMethod(type,
                    () => new AzureAuthentication<AzureAuthSettings>(authSection.Get<AzureAuthSettings>(), configuration));

            default:
                throw new Exception($"No implementation found for auth method '{type}'. " +
                                        "Update authentication type or add a new implementation");
        }
    }

    private static IAuthenticationMethod InitializeAuthenticationMethod(string authenticationType, Func<IAuthenticationMethod> initAuthMethod)
    {
        methods.TryGetValue(authenticationType ?? string.Empty, out var method);
        if (method == null)
        {
            method = initAuthMethod();
            methods.Add(authenticationType ?? string.Empty, method);
        }

        return method;
    }
}
