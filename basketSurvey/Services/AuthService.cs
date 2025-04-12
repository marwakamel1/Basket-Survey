using basketSurvey.Contracts.Users;
using basketSurvey.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace basketSurvey.Services
{
    public class AuthService
        (UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtProvider jwtProvider,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor,
        IEmailSender emailSender) : IAuthService
    {
        public UserManager<ApplicationUser> UserManager { get; } = userManager;
        public RoleManager<IdentityRole> RoleManager { get; } = roleManager;
        public IJwtProvider JwtProvider { get; } = jwtProvider;
        private readonly int _refreshTokenExpiryDays = 14;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IEmailSender _emailSender = emailSender;
        //public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
        //{
        //    var user = await UserManager.FindByEmailAsync(request.email);

        //    if (user == null)
        //    {

        //        return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        //    }

        //    bool passIsValid = await UserManager.CheckPasswordAsync(user, request.password);

        //    if (!passIsValid)
        //    {

        //        return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        //    }

        //    var (token, expiresIn) = JwtProvider.GenerateToken(user);
        //    string refreshToken = GenerateRefreshToken();
        //    var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
        //    user.RefreshTokens.Add(new RefreshToken
        //    {
        //        Token = refreshToken,
        //        ExpiresOn = refreshTokenExpiration
        //    });

        //    await UserManager.UpdateAsync(user);
        //    var response = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
        //    return Result.Success(response);
        //}

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await UserManager.FindByEmailAsync(email);

            if (user == null)
            {

                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            }

            //bool passIsValid = await UserManager.CheckPasswordAsync(user, password);
            //if (!passIsValid)
            //{

            //    return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            //}

            var result = await _signInManager.PasswordSignInAsync(user,password,false,false);
            if (result.Succeeded) {
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

             return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailIsNotConfirmed :  UserErrors.InvalidCredentials);

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
            //var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
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

        public async Task<Result> RegisterAsync(RegisterRequest model, CancellationToken cancellationToken = default)
        {
            try
            {
                if (await UserManager.FindByEmailAsync(model.Email) is not null)
                {
                    return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);
                }

                var user = model.Adapt<ApplicationUser>();


                var result = await UserManager.CreateAsync(user, model.Password);
               
                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);

                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    _logger.LogInformation($"confirmation code : {code}", code);

                    await SendConfirmationEmail(user, code);

                    return Result.Success();

                }

                var error = result.Errors.First();
                //foreach (var error in result.Errors)
                //{
                //    errors += $"{error.Description},";
                //}
                //new Error("CreationFailed",errors)
                return Result.Failure<AuthResponse>(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
                //await UserManager.AddToRoleAsync(user, "User");

                //var jwtSecurityToken = await CreateJwtToken(user);
                //var (token, expiresIn) = JwtProvider.GenerateToken(user);
                //string refreshToken = GenerateRefreshToken();
                //var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
                //user.RefreshTokens.Add(new RefreshToken
                //{
                //    Token = refreshToken,
                //    ExpiresOn = refreshTokenExpiration
                //});

                //await UserManager.UpdateAsync(user);
                //var response = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
                //return Result.Success(response);
            }
            catch (Exception ex)
            {
                return null;
            }
            //return null;
        }

        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var user = await UserManager.FindByIdAsync(request.UserId);
            if (user is null)
                return Result.Failure(UserErrors.InvalidCode);

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = request.Code;
            try
            {
                 code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            }
            catch(FormatException ex)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }

            var result = await UserManager.ConfirmEmailAsync(user, code);

            if(result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();
         
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
    
        public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
        {
            var user = await UserManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation($"confirmation code : {code}", code);

            await SendConfirmationEmail(user, code);

            return Result.Success();

        }

        public async Task<bool> SendResetPasswordEmailAsync(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);

            if (user == null)
               return Result.Success();

            var code = await UserManager.GeneratePasswordResetTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //_logger.LogInformation($"confirmation test : {code}", code);

            _logger.LogInformation($"Reset password email sent to {user.Email}");
            await SendResetPasswordEmail(user, code);

            return Result.Success();

        }
        private async Task SendConfirmationEmail(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
                templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
                }
            );

            BackgroundJob.Enqueue(() =>  _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody));

            await Task.CompletedTask;
        }
        private async Task SendResetPasswordEmail(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
                templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
                }
            );

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Change Password", emailBody));

            await Task.CompletedTask;
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await UserManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);


            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                 result = await UserManager.ResetPasswordAsync(user, code, request.NewPassword);

            }
            catch (FormatException ex)
            {
                result = IdentityResult.Failed(UserManager.ErrorDescriber.InvalidToken());
            }


            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
        }


    }
}
