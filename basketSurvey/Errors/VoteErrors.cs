namespace basketSurvey.Errors
{
    public static class VoteErrors
    {
        public static readonly Error  VoteDuplicated = new Error("VoteDuplicated", "User has voted for this poll before", StatusCodes.Status409Conflict);
        public static readonly Error VoteInvalidQuestions = new Error("VoteInvalidQuestions", "Vote Invalid Questions", StatusCodes.Status400BadRequest);
    }
}
