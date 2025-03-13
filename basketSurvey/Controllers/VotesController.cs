using basketSurvey.Contracts.Votes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace basketSurvey.Controllers
{
    [Route("api/polls/{pollId}/vote")]
    [ApiController]
    [Authorize]
    public class VotesController(IQuestionService questionService ,IVoteService voteService) : ControllerBase
    {
        public IQuestionService _questionService  = questionService;
        private readonly IVoteService _voteService = voteService;

        [HttpGet]
        public async Task<IActionResult> Start([FromRoute] int  pollId , CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _questionService.GetAvailableAsync(pollId , userId!, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost]
        public async Task<IActionResult> Vote([FromRoute] int pollId, VoteRequest request , CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _voteService.AddAsync(pollId, userId!, request, cancellationToken);

            return result.IsSuccess ? Created() :
                result.ToProblem();
        }
    }
}
