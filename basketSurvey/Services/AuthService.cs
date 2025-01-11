using basketSurvey.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace basketSurvey.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtProvider jwtProvider) : IAuthService
    {
        public UserManager<ApplicationUser> UserManager { get; } = userManager;
        public RoleManager<IdentityRole> RoleManager { get; } = roleManager;
        public IJwtProvider JwtProvider { get; } = jwtProvider;
        private readonly int _refreshTokenExpiryDays = 14;
        public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await UserManager.FindByEmailAsync(request.email);

            if (user == null)
            {

                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            }

            bool passIsValid = await UserManager.CheckPasswordAsync(user, request.password);

            if (!passIsValid)
            {

                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            }

            var (token, expiresIn) = JwtProvider.GenerateToken(user);
            string refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await UserManager.UpdateAsync(user);
            var response = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
            return Result.Success(response);
        }

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            //decode/validate the token by the key , return userid from claims
            var userId = JwtProvider.ValidateToken(token);

            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

            var user = await UserManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            //check if user has active refresh token 
            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

            //revoke refresh token
            userRefreshToken.RevokedOn = DateTime.UtcNow;
            // generate new token and refresh token
            var (newToken, expiresIn) = JwtProvider.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = refreshTokenExpiration
            });
            // save ref. token in db
            await UserManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);
            return Result.Success(response);
        }


        public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = JwtProvider.ValidateToken(token);

            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

            var user = await UserManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            await UserManager.UpdateAsync(user);

            return Result.Success();
        }
        public static string GenerateRefreshToken()
        {

            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest model, CancellationToken cancellationToken = default)
        {
            try
            {
                if (await UserManager.FindByEmailAsync(model.Email) is not null)
                {
                    return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
                }
                if (await UserManager.FindByNameAsync(model.Username) is not null)
                {
                    return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    //PasswordHash = model.Password
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {

                    var errors = string.Empty;
                    foreach (var error in result.Errors)
                    {
                        errors += $"{error.Description},";
                    }

                    return Result.Failure<AuthResponse>(new Error("CreationFailed",errors));

                }


                //await UserManager.AddToRoleAsync(user, "User");

                //var jwtSecurityToken = await CreateJwtToken(user);
                var (token, expiresIn) = JwtProvider.GenerateToken(user);
                string refreshToken = GenerateRefreshToken();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpiration
                });

                await UserManager.UpdateAsync(user);
                var response = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
                return Result.Success(response);
            }
            catch (Exception ex)
            {
                return null;
            }
            //return null;
        }


    }
}
