

namespace basketSurvey.Errors
{
    public static class UserErrors
    {
        public static readonly Error InvalidCredentials = new Error("InvalidCredentials", "Invalid Email/Password");
        public static readonly Error InvalidToken = new Error("InvalidToken", "Invalid Token");
        
    }
}
