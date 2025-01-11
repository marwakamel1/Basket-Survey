using basketSurvey.Contracts.Polls;
using FluentValidation;

namespace basketSurvey.Contracts.Users
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {

        public LoginRequestValidator()
        {
            RuleFor(x => x.email).NotEmpty().EmailAddress();
            RuleFor(x => x.password).NotEmpty();
        }
       
    }
}
