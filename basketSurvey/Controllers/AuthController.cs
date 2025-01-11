using basketSurvey.Abstractions;
using basketSurvey.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace basketSurvey.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService authService = authService;

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request , CancellationToken cancellationToken )
        {
            //throw new Exception("My exception");
           var authResult =  await authService.GetTokenAsync(request, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value)
                : authResult.ToProblem(StatusCodes.Status404NotFound);
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var authResult = await authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return authResult.IsSuccess ? Ok(authResult.Value)
                :  authResult.ToProblem(StatusCodes.Status404NotFound);
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description) ;
        }

        [HttpPost]
        [Route("RevokeToken")]
        public async Task<IActionResult> RevokeToken(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var authResult = await authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return authResult.IsSuccess ?  Ok()
                  : authResult.ToProblem(StatusCodes.Status404NotFound);
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var authResult = await authService.RegisterAsync(request, cancellationToken);

            return authResult.IsSuccess ? Ok(authResult.Value)
                  : authResult.ToProblem(StatusCodes.Status404NotFound);
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: authResult.Error.code, detail: authResult.Error.description) ;
        }
    }
}
