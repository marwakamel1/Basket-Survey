namespace basketSurvey.Contracts.Users
{
    public record ConfirmEmailRequest
    (
        string UserId,
        string Code
        );
    
}
