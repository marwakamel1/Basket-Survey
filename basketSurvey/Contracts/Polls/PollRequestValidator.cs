using FluentValidation;

namespace basketSurvey.Contracts.Polls

{
    public class PollRequestValidator : AbstractValidator<PollRequest>
    {
        public PollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty();

            RuleFor(x => x.StartAt)
                .NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now));

            RuleFor(x => x)
                .Must(ValidDate)
                .WithName(nameof(PollRequest.EndAt))
                .WithMessage("{PropertyName} must be greater than or equal start date");


        }

        private bool ValidDate(PollRequest request)
        {

            return request.EndAt >= request.StartAt;
        }
    }
}
