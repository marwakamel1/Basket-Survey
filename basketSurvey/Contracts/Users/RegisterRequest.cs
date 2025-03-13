namespace basketSurvey.Contracts.Users
{
    public record RegisterRequest
    ( 
        //string Username,
        string Email,
        string FirstName,
        string LastName,
        string Password
        );
}
