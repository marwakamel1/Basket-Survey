using basketSurvey.Contracts.Results;
using System.Linq;

namespace basketSurvey.Services
{
    public class ResultService(ApplicationDbContext context) : IResultService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken)
        {
            var pollVotesResult = await _context.Polls.Where(x => x.Id == pollId)
                .Select(
                 x => new PollVotesResponse
                 (
                     x.Title,
                     x.Votes.Select( v => new VoteResponse(
                          $"{v.User.FirstName} {v.User.LastName}",
                          v.SubmittedOn,
                         v.VoteAnswers.Select(answer => new QuestionAnswerResponse(answer.Question.Content , answer.Answer.Content))
                         ))

                 )
                ).SingleOrDefaultAsync(cancellationToken);

            return pollVotesResult is null ? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound) : Result.Success(pollVotesResult);
        }

        public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken) {

            bool pollExist = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
            if (!pollExist)
                return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

            var votesPerDay = await _context.Votes.Where(x => x.PollId == pollId)
                .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmittedOn) })
                .Select(g => new VotesPerDayResponse(g.Key.Date, g.Count()))
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);



        }

        public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionsAsync(int pollId, CancellationToken cancellationToken)
        {

            bool pollExist = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
            if (!pollExist)
                return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

            var votesPerQuestion = await _context.VoteAnswers.Where(x => x.Question.PollId == pollId)
                //.GroupBy(x => new { Content = x.Question.Content })
                .Select(x => new VotesPerQuestionResponse(x.Question.Content,
                x.Question.VoteAnswers.GroupBy(x => new { AnswerId = x.AnswerId ,AnswerContent = x.Answer.Content})
                .Select(g => new VotesPerAnswerResponse(g.Key.AnswerContent , g.Count()))
                ))
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);



        }
    }
}
