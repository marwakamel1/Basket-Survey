﻿

namespace basketSurvey.Services
{
    public interface IPollService
    {
        Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<PollResponse>> AddAsync(PollRequest poll, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default);
        Task<Result>  DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default);
    }
}
