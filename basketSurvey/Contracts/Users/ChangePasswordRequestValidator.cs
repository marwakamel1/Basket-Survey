using basketSurvey.Abstractions.Const;
using FluentValidation;

namespace basketSurvey.Contracts.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();

            RuleFor(x => x.NewPassword).NotEmpty().Matches(RegexPatterns.password)
               .WithMessage("password must contain letters,numbers")
               .NotEqual(x => x.CurrentPassword)
               .WithMessage("password must not match");


        }
       
    }
}
