using basketSurvey.Contracts.Questions;

namespace basketSurvey.Services
{
    public interface IQuestionService
    {
        public Task<Result<QuestionResponse>> GetAsync(int pollId, int id ,CancellationToken cancellationToken);
        public Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken);
        public Task<Result<QuestionResponse>> AddAsync(int pollId,QuestionRequest request,CancellationToken cancellationToken);
        public Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default);
        public Task<Result> ToggleStatusAsync(int pollId , int id, CancellationToken cancellationToken = default);
        public Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default);

    }
}
