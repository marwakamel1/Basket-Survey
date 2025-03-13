using basketSurvey.Contracts.Questions;
using Microsoft.Extensions.Caching.Hybrid  ;

namespace basketSurvey.Services
{
    public class QuestionService(ApplicationDbContext applicationDbContext , HybridCache hybridCache) : IQuestionService
    {
        public ApplicationDbContext _context = applicationDbContext;
        private readonly HybridCache _hybridCache = hybridCache;
        private const string _cashPrefix = "availableQuestions";
        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken)
        {
            var question = await _context.Questions.Where(x => x.PollId == pollId && x.Id == id)
                .Include(x => x.Answers)
                .ProjectToType<QuestionResponse>()
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (question is null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

            return Result.Success(question);
        }



        public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                bool pollExist = await _context.Polls.AnyAsync(x => x.Id == pollId);

                if (pollExist)
                {
                    bool questionExist = await _context.Questions.AnyAsync(x => x.PollId == pollId && x.Content == request.Content);

                    if (questionExist)
                        return Result.Failure<QuestionResponse>(QuestionErrors.QuestionDuplicatedContent);

                    var question = request.Adapt<Question>();
                    //foreach (var answer in request.Answers)
                    //{
                    //    question.Answers.Add(new Answer { Content = answer });
                    //}
                    question.PollId = pollId;
                    await _context.Questions.AddAsync(question, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    await _hybridCache.RemoveAsync($"{_cashPrefix}-{pollId}",cancellationToken);
                    return Result.Success(question.Adapt<QuestionResponse>());
                }
                else
                {
                    return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken)
        {
            bool pollExist = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);
            if (!pollExist)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

            var questions = await _context.Questions.Where(x => x.PollId == pollId).Include(x => x.Answers)
                .ProjectToType<QuestionResponse>() // used to select certain columns instead of all columns // better performance// mapster method
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            return Result.Success<IEnumerable<QuestionResponse>>(questions);
        }

        public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            bool questionExist = await _context.Questions.AnyAsync(x => x.PollId == pollId && x.Id != id && x.Content == request.Content, cancellationToken: cancellationToken);
            if (questionExist)
                return Result.Failure(QuestionErrors.QuestionDuplicatedContent);

            var question = await _context.Questions.Where(x => x.PollId == pollId && x.Id == id ).Include(x => x.Answers).SingleOrDefaultAsync();
            if(question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.Content = request.Content;
            var alreadyExistAnswers = question.Answers.Select(answer => answer.Content);
            var newAnswers = request.Answers.Where(answer => !alreadyExistAnswers.Contains(answer));
            //var newAnswes = request.Answers.Except(alreadyExistAnswers);

            foreach (var answer in newAnswers)
            {
                question.Answers.Add(new Answer { Content = answer });
            }

            question.Answers.ToList().ForEach(answer =>
            {
                answer.IsActive = request.Answers.Contains(answer.Content);
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _hybridCache.RemoveAsync($"{_cashPrefix}-{pollId}", cancellationToken);

            return Result.Success(question);

        }
        public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken: cancellationToken);
            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);
            question.IsActive = !question.IsActive;
            await _context.SaveChangesAsync(cancellationToken);

            await _hybridCache.RemoveAsync($"{_cashPrefix}-{pollId}", cancellationToken);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId,string userId ,CancellationToken cancellationToken)
        {
          

            //bool hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken: cancellationToken);
            //if (hasVote)
            //    return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.VoteDuplicated);

            //bool pollFound = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndAt >= DateOnly.FromDateTime(DateTime.UtcNow));

            //if(!pollFound)
            //    return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
            var cacheKey = $"{_cashPrefix}-{pollId}";

            var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(cacheKey, async cacheEntry =>

             await _context.Questions.Where(x => x.PollId == pollId && x.IsActive)
                .Include(x => x.Answers)
                .AsNoTracking()
                .Select(x => new QuestionResponse(
                    x.Id,
                    x.Content,
                    x.Answers.Where(a => a.IsActive).Select(a => new Contracts.Answers.AnswerResponse(a.Id, a.Content))

                    ))
                .ToListAsync(cancellationToken)
                , cancellationToken: cancellationToken);
            

            return Result.Success(questions);
        }
    }
}
