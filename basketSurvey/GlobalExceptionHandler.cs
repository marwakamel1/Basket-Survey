using Microsoft.AspNetCore.Diagnostics;

namespace basketSurvey
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public ILogger<GlobalExceptionHandler> _logger { get; } = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception,"Something went wrong : {Message}",exception.Message);

            var problemDetails = new ProblemDetails { 
            
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Type = ""
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problemDetails);
            return true;
        }
    }
}
