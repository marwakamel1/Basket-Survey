namespace basketSurvey.Contracts.Votes
{
    public record VoteRequest
    (
     IEnumerable<VoteAnswerRequest> Answers
    );
}
