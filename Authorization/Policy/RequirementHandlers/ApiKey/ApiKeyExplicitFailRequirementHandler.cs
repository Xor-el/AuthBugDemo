using AuthBugDemo.Authorization.Policy.RequirementHandlerValidators.ApiKey;
using AuthBugDemo.Authorization.Policy.Requirements.ApiKey;
using Microsoft.AspNetCore.Authorization;

namespace AuthBugDemo.Authorization.Policy.RequirementHandlers.ApiKey
{
    public class ApiKeyExplicitFailRequirementHandler : AuthorizationHandler<ApiKeyExplicitFailRequirement>
    {
        private readonly IApiKeyRequirementHandlerValidator _validator;

        public ApiKeyExplicitFailRequirementHandler(IApiKeyRequirementHandlerValidator validator)
        {
            _validator = validator;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ApiKeyExplicitFailRequirement requirement)
        {
            var validationResponse = _validator.ValidateApiKey(context);

            if (validationResponse.IsSuccessful)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, validationResponse.FailureReason!));
            }

            return Task.CompletedTask;
        }
    }
}