using basketSurvey.Abstractions.Const;
using FluentValidation;

namespace basketSurvey.Contracts.Users
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Matches(RegexPatterns.password)
                .WithMessage("password must contain letters,numbers");
            RuleFor(x => x.FirstName).NotEmpty().Length(3,100);
            RuleFor(x => x.LastName).NotEmpty().Length(3, 100);
        }
    }
}
