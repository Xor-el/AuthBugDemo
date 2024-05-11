using AuthBugDemo.Models.Authentication.ApiKey;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AuthBugDemo.Authentication.ApiKey.Handlers
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IOptionsMonitor<ApiKeyAuthenticationOptions> _options;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
                                           ILoggerFactory loggerFactory,
                                           UrlEncoder encoder) : base(options, loggerFactory, encoder)
        {
            _options = options;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string headerName = _options.CurrentValue.HeaderName!;

            if (!Request.Headers.TryGetValue(headerName, out var requestApiKey))
            {
                return AuthenticateResult.Fail($"No API key found in the '{headerName}' header.");
            }

            // Ensure only one API key is provided, otherwise return 'Fail' with an error message.
            if (requestApiKey.Count > 1)
            {
                return AuthenticateResult.Fail("Multiple API keys found in header. Please provide only one key.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.AuthenticationMethod, Scheme.Name)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();

            Response.Headers.Append(HeaderNames.WWWAuthenticate, Scheme.Name);

            //if (authResult.Failure is not null && !string.IsNullOrEmpty(authResult.Failure.Message))
            //{

            //}

            await base.HandleChallengeAsync(properties);
        }
    }
}