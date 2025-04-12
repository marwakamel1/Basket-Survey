using basketSurvey.Abstractions.Const;
using FluentValidation;

namespace basketSurvey.Contracts.Users
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.NewPassword).NotEmpty().Matches(RegexPatterns.password)
                .WithMessage("password must contain letters,numbers");
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
