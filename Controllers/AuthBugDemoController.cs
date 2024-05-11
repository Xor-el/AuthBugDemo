using AuthBugDemo.Models.Authentication.ApiKey;
using AuthBugDemo.Models.Authorization.ApiKey;
using AuthBugDemo.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthBugDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthBugDemoController : ControllerBase
    {

        private readonly ILogger<AuthBugDemoController> _logger;

        public AuthBugDemoController(ILogger<AuthBugDemoController> logger)
        {
            _logger = logger;
        }

        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme, Policy = ApiKeyPolicies.ImplicitFail)]
        [HttpPost("implicit-fail")]
        public IActionResult ImplicitFail([FromBody] AuthBugRequest request)
        {
            return Ok();
        }

        [Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme, Policy = ApiKeyPolicies.ExplicitFail)]
        [HttpPost("explicit-fail")]
        public IActionResult ExplicitFail([FromBody] AuthBugRequest request)
        {
            return Ok();
        }
    }
}
