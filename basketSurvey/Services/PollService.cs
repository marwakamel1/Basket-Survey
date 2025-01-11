using basketSurvey.Entities;

namespace basketSurvey.Services
{
    public class PollService(ApplicationDbContext context) : IPollService
    {
        private readonly ApplicationDbContext _context = context;
        //private static readonly List<Poll> _polls = [
        //new Poll
        //{
        //    Id = 1,
        //    Title = "Poll 1",
        //    Summary = "My first poll"
        //}
        //];

        public async Task<Result<PollResponse>> AddAsync(PollRequest poll, CancellationToken cancellationToken = default)
        {

            bool isExistingTitle = await _context.Polls.AnyAsync(x => x.Title == poll.Title);
            if (isExistingTitle)
                return Result.Failure<PollResponse>(PollErrors.PollDuplicatedTitle);
            await _context.AddAsync(poll.Adapt<Poll>(), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(poll.Adapt<PollResponse>());
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _context.Polls.FindAsync(id, cancellationToken);
            if (poll is null)
            {
                return Result.Failure(PollErrors.PollNotFound);
            }

            _context.Remove(poll);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
        {

            var poll = await _context.Polls.FindAsync(id, cancellationToken);

            return poll is null ? Result.Failure<PollResponse>(PollErrors.PollNotFound) : Result.Success(poll.Adapt<PollResponse>());

        }


        public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken) => Result.Success( (await _context.Polls.AsNoTracking().ToListAsync(cancellationToken)).Adapt<IEnumerable<PollResponse>>());

        public async Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default)
        {
            bool isExistingTitle = await _context.Polls.AnyAsync(x => x.Title == poll.Title && x.Id != id, cancellationToken: cancellationToken);
            if (isExistingTitle)
                return Result.Failure<PollResponse>(PollErrors.PollDuplicatedTitle);

            var foundpoll = await _context.Polls.FindAsync(id, cancellationToken);
            if (foundpoll is null)
                return Result.Failure(PollErrors.PollNotFound);
            foundpoll.Title = poll.Title;
            foundpoll.Summary = poll.Summary;
            foundpoll.IsPublished = poll.IsPublished;
            foundpoll.StartAt = poll.StartAt;
            foundpoll.EndAt = poll.EndAt;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
        public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
        {
            var foundpoll = await _context.Polls.FindAsync(id, cancellationToken);
            if (foundpoll is null)
                return Result.Failure(PollErrors.PollNotFound);
            foundpoll.IsPublished = !foundpoll.IsPublished;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }

    }

}
