using Microsoft.AspNetCore.Authorization;

namespace AuthBugDemo.Authorization.Policy.Requirements.ApiKey
{
    public abstract class BaseApiKeyRequirement : IAuthorizationRequirement { }
}
