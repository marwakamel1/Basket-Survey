using FluentValidation;

namespace basketSurvey.Models
{
    public class CreatePollRequestValidator : AbstractValidator<PollRequest>
    {
        public CreatePollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty();
        }
    }
}
