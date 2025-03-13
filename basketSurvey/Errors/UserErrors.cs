

namespace basketSurvey.Errors
{
    public static class UserErrors
    {
        public static readonly Error InvalidCredentials = new Error("InvalidCredentials", "Invalid Email/Password", StatusCodes.Status401Unauthorized);
        public static readonly Error InvalidToken = new Error("InvalidToken", "Invalid Token", StatusCodes.Status401Unauthorized);
        public static readonly Error DuplicatedEmail = new Error("DuplicatedEmail", "same user with this email already exist", StatusCodes.Status409Conflict);
        public static readonly Error EmailIsNotConfirmed = new Error("EmailIsNotConfirmed", "Email Is Not Confirmed", StatusCodes.Status401Unauthorized);
        public static readonly Error InvalidCode = new Error("InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);
        public static readonly Error DuplicatedConfirmation = new Error("DuplicatedConfirmation", "Duplicated Confirmation", StatusCodes.Status409Conflict);

    }
}
