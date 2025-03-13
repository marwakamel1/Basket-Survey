using basketSurvey.Contracts.Votes;

namespace basketSurvey.Services
{
    public interface IVoteService
    {
        public Task<Result> AddAsync(int pollId , string userId, VoteRequest request , CancellationToken cancellationToken);
    }
}
