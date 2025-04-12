using basketSurvey.Abstractions;
using basketSurvey.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace basketSurvey.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IAuthService authService = authService;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request , CancellationToken cancellationToken )
        {
            _logger.LogInformation("login request with email : {email}", request.email);
            //throw new Exception("My exception");
           var authResult =  await authService.GetTokenAsync(request.email,request.password, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value)
                : authResult.ToProblem();
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var authResult = await authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return authResult.IsSuccess ? Ok(authResult.Value)
                :  authResult.ToProblem();
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description) ;
        }

        [HttpPost]
        [Route("RevokeToken")]
        public async Task<IActionResult> RevokeToken(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var authResult = await authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return authResult.IsSuccess ?  Ok()
                  : authResult.ToProblem();
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var authResult = await authService.RegisterAsync(request, cancellationToken);

            return authResult.IsSuccess ? Ok()
                  : authResult.ToProblem();
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description) ;
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            var authResult = await authService.ConfirmEmailAsync(request);

            return authResult.IsSuccess ? Ok()
                  : authResult.ToProblem();
            //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description) ;
        }

        [HttpPost]
        [Route("ResendConfirmationEmail")]
        public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
        {
            var authResult = await authService.ResendConfirmationEmailAsync(request);

            return authResult.IsSuccess ? Ok()
                  : authResult.ToProblem();
            //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description) ;
        }

        [HttpPost]
        [Route("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var authResult = await authService.SendResetPasswordEmailAsync(request.email);

            return Ok();
                
            //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description) ;
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var authResult = await authService.ResetPasswordAsync(request);

            return authResult.IsSuccess ? Ok()
                 : authResult.ToProblem();
        }
    }
}
