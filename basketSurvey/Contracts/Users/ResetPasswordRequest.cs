namespace basketSurvey.Contracts.Users
{
    public record ResetPasswordRequest
       (
        string Email,
        string Code,
        string NewPassword
        );
}
