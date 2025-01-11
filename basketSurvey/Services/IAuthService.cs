using basketSurvey.Contracts.Users;

namespace basketSurvey.Services
{
    public interface IAuthService
    {
        public  Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);

        public  Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

        public  Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

        public  Task<Result<AuthResponse>> RegisterAsync(RegisterRequest model, CancellationToken cancellationToken = default);
    }
}
