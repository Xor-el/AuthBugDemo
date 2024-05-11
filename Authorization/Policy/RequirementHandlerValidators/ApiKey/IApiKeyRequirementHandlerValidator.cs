using AuthBugDemo.Authorization.Policy.RequirementHandlerValidators.Response;
using Microsoft.AspNetCore.Authorization;

namespace AuthBugDemo.Authorization.Policy.RequirementHandlerValidators.ApiKey
{
    public interface IApiKeyRequirementHandlerValidator
    {
        RequirementHandlerValidatorResponse ValidateApiKey(AuthorizationHandlerContext context);
    }
}
