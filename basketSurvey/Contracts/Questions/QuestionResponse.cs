using basketSurvey.Contracts.Answers;

namespace basketSurvey.Contracts.Questions
{
    public record QuestionResponse
    (
        int Id,
        string Content,
        IEnumerable<AnswerResponse> Answers
    );

}
