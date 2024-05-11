namespace AuthBugDemo.Authorization.Policy.RequirementHandlerValidators.Response
{
    public class RequirementHandlerValidatorResponse
    {
        public bool IsSuccessful { get; set; }
        public string? FailureReason { get; set; }
    }
}
