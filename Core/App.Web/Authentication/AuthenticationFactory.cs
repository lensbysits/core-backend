using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Lens.Core.App.Web.Authentication
{
    internal static class AuthenticationFactory
    {
        private static readonly Dictionary<string, IAuthententicationMethod> methods = new Dictionary<string, IAuthententicationMethod>();
        public static IAuthententicationMethod GetAuthenticationMethod(IConfiguration configuration)
        {
            var authSettings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
            if (authSettings != null && string.IsNullOrEmpty(authSettings.AuthenticationType))
            {
                authSettings.AuthenticationType = "oauth2";
            }

            return GetAuthenticationMethodByType(authSettings, configuration);
        }

        private static IAuthententicationMethod GetAuthenticationMethodByType(AuthSettings authSettings, IConfiguration configuration)
        {
            if (authSettings == null)
            {
                return InitializeAuthenticationMethod(AuthenticationMethod.Anonymous, () => new AnonymousAuthentication());
            }

            switch (authSettings.AuthenticationType.ToLowerInvariant())
            {
                case AuthenticationMethod.OAuth2:
                    return InitializeAuthenticationMethod(authSettings.AuthenticationType, () => new OAuth2Authentication(authSettings));

                case AuthenticationMethod.ApiKey:
                    return InitializeAuthenticationMethod(authSettings.AuthenticationType, () => new ApiKeyAuthentication(authSettings));

                case AuthenticationMethod.AzureAd:
                    return InitializeAuthenticationMethod(authSettings.AuthenticationType, () => new AzureAuthentication(authSettings, configuration));

                default:
                    throw new Exception($"No implementation found for auth method '{authSettings.AuthenticationType.ToLowerInvariant()}'. " +
                                            "Update authentication type or add a new implementation");
            }
        }

        private static IAuthententicationMethod InitializeAuthenticationMethod(string authenticationType, Func<IAuthententicationMethod> initAuthMethod)
        {
            methods.TryGetValue(authenticationType ?? string.Empty, out var method);
            if (method == null)
            {
                method = initAuthMethod();
                methods.Add(authenticationType, method);
            }

            return method;
        }
    }
}
