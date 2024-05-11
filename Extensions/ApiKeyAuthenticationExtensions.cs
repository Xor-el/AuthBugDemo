using AuthBugDemo.Authentication.ApiKey.Handlers;
using AuthBugDemo.Models.Authentication.ApiKey;
using Microsoft.AspNetCore.Authentication;

namespace AuthBugDemo.Extensions
{
    public static class ApiKeyAuthenticationExtensions
    {
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options)
        {
            return authenticationBuilder.AddApiKey(ApiKeyAuthenticationDefaults.AuthenticationScheme, options);
        }

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder authenticationBuilder, string authenticationScheme, Action<ApiKeyAuthenticationOptions> options)
        {
            return authenticationBuilder.AddApiKey(authenticationScheme, null, options);
        }

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder authenticationBuilder, string authenticationScheme, string? displayName, Action<ApiKeyAuthenticationOptions> options)
        {
            authenticationBuilder.Services.Configure(options);
            return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(authenticationScheme, displayName, options);
        }
    }
}
