using FluentValidation;

namespace basketSurvey.Contracts.Users
{
    public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
    {

        public ResendConfirmationEmailRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
       
    }
    
}
