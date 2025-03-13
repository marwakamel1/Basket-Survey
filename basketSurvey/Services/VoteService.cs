using basketSurvey.Contracts.Questions;
using basketSurvey.Contracts.Votes;
using Microsoft.EntityFrameworkCore;

namespace basketSurvey.Services
{
    public class VoteService(ApplicationDbContext context) : IVoteService
    {
        public ApplicationDbContext _context = context;

        public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken)
        {
            bool hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken: cancellationToken);
            if (hasVote)
                return Result.Failure(VoteErrors.VoteDuplicated);

            bool pollFound = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken: cancellationToken);

            if (!pollFound)
                return Result.Failure(PollErrors.PollNotFound);

            var availableQuestions = await _context.Questions.Where(x => x.PollId == pollId && x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken: cancellationToken);

            if(!request.Answers.Select(x => x.QuestionId).SequenceEqual(availableQuestions))
                return Result.Failure(VoteErrors.VoteInvalidQuestions);

            var vote = new Vote()
            {

                PollId = pollId,
                UserId = userId,
                VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
            };

            await _context.Votes.AddAsync(vote,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(vote);
        }
    }
}
