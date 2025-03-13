﻿namespace basketSurvey.Abstractions
{
    public static class ResultExtensions
    {

        public static ObjectResult ToProblem(this Result result )
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("invaliddddddddd");

            var problem = Results.Problem(statusCode :  result.Error.statusCode);

            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

            problemDetails!.Extensions = new Dictionary<string, object?>
            {
                {
                    "errors" , new []{result.Error.code , result.Error.description }
                }
            };

            return new ObjectResult(problemDetails);
        } 
    }
}
