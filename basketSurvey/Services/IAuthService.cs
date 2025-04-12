using basketSurvey.Contracts.Users;

namespace basketSurvey.Services
{
    public interface IAuthService
    {
        public  Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);

        public  Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

        public  Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

        public  Task<Result> RegisterAsync(RegisterRequest model, CancellationToken cancellationToken = default);

        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);

        Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);

        Task<Result> SendResetPasswordEmailAsync(string email);

        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
