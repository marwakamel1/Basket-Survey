using basketSurvey.Contracts.Users;

namespace basketSurvey.Services
{
    public interface IUserService
    {
        public  Task<Result<UserProfileResponse>> GetProfileAsync(string userId);

        public  Task<Result> UpdateInfoAsync(string userId, UserUpdateRequest request);

        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    }
}
