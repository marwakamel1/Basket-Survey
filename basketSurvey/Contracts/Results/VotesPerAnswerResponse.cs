namespace basketSurvey.Contracts.Results
{
    public record VotesPerAnswerResponse
    (
        string Answer,
        int NumberOfVotes
        );
    
}
