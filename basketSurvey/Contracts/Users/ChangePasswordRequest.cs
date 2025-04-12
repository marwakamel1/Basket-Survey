namespace basketSurvey.Contracts.Users
{
    public record ChangePasswordRequest
   ( string CurrentPassword ,
     string NewPassword
    );
}
