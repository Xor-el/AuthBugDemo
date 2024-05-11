using AuthBugDemo.Authorization.Policy.RequirementHandlerValidators.ApiKey;
using AuthBugDemo.Authorization.Policy.Requirements.ApiKey;
using Microsoft.AspNetCore.Authorization;

namespace AuthBugDemo.Authorization.Policy.RequirementHandlers.ApiKey
{
    public class ApiKeyImplicitFailRequirementHandler : AuthorizationHandler<ApiKeyImplicitFailRequirement>
    {
        private readonly IApiKeyRequirementHandlerValidator _validator;

        public ApiKeyImplicitFailRequirementHandler(IApiKeyRequirementHandlerValidator validator)
        {
            _validator = validator;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ApiKeyImplicitFailRequirement requirement)
        {
            var validationResponse = _validator.ValidateApiKey(context);

            if (validationResponse.IsSuccessful)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}