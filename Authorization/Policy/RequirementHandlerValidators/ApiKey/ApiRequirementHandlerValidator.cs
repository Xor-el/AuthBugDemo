using AuthBugDemo.Authorization.Policy.RequirementHandlerValidators.Response;
using AuthBugDemo.Models.Authentication.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AuthBugDemo.Authorization.Policy.RequirementHandlerValidators.ApiKey
{
    public class ApiRequirementHandlerValidator : IApiKeyRequirementHandlerValidator
    {
        private readonly IOptionsMonitor<ApiKeyAuthenticationOptions> _options;

        public ApiRequirementHandlerValidator(IOptionsMonitor<ApiKeyAuthenticationOptions> options)
        {
            _options = options;
        }

        public RequirementHandlerValidatorResponse ValidateApiKey(AuthorizationHandlerContext context)
        {
            if (!context.User.Identity?.IsAuthenticated ?? false)
            {
                return new RequirementHandlerValidatorResponse
                {
                    FailureReason = "User Not Authenticated",
                    IsSuccessful = false
                };
            }

            if (context.Resource is not HttpContext { Request: not null } httpContext)
            {
                return new RequirementHandlerValidatorResponse
                {
                    FailureReason = "Unable to Retrieve HttpContext",
                    IsSuccessful = false
                };
            }

            var headerName = _options.CurrentValue.HeaderName!;

            if (!httpContext.Request.Headers.TryGetValue(headerName, out var requestApiKey))
            {
                return new RequirementHandlerValidatorResponse
                {
                    FailureReason = $"No API key found in the '{headerName}' header.",
                    IsSuccessful = false
                };
            }

            var apiKey = _options.CurrentValue.Key!;

            if (!string.Equals(apiKey, requestApiKey, StringComparison.Ordinal))
            {
                return new RequirementHandlerValidatorResponse
                {
                    FailureReason = "Invalid API Key",
                    IsSuccessful = false
                };
            }

            return new RequirementHandlerValidatorResponse
            {
                IsSuccessful = true
            };
        }
    }
}
