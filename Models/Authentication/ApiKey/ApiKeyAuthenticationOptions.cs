using Microsoft.AspNetCore.Authentication;

namespace AuthBugDemo.Models.Authentication.ApiKey
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string? HeaderName { get; set; }

        public string? Key { get; set; }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(HeaderName))
            {
                throw new ArgumentException("API Key Header Name must be provided.");
            }

            if (string.IsNullOrEmpty(Key))
            {
                throw new ArgumentException("API Key must be provided.");
            }
        }
    }
}
