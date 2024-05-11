using AuthBugDemo.Authorization.Policy.Requirements.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace AuthBugDemo.Authorization
{
    public class AuthorizationResultTransformer : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(RequestDelegate requestDelegate,
                                      HttpContext httpContext,
                                      AuthorizationPolicy authorizationPolicy,
                                      PolicyAuthorizationResult policyAuthorizationResult)
        {
            if (policyAuthorizationResult.Forbidden)
            {
                var authSchemes = authorizationPolicy.AuthenticationSchemes;

                foreach (var authScheme in authSchemes)
                {
                    httpContext.Response.Headers.Append(HeaderNames.WWWAuthenticate, authScheme);
                }

                var authorizationFailure = policyAuthorizationResult.AuthorizationFailure;

                var failedRequirements = authorizationFailure?.FailedRequirements.ToList() ?? Enumerable.Empty<IAuthorizationRequirement>().ToList();

                var failureReasons = authorizationFailure?.FailureReasons.ToList() ?? Enumerable.Empty<AuthorizationFailureReason>().ToList();

                if (failedRequirements.OfType<BaseApiKeyRequirement>().Any())
                {
                    // Return a 401 Unauthorized response instead of 403 Forbidden response.
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }

            await _defaultHandler.HandleAsync(requestDelegate, httpContext, authorizationPolicy, policyAuthorizationResult);
        }
    }
}
