﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace basketSurvey.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        [HttpGet("row-data")]
        public async Task<IActionResult> GetPollVotes([FromRoute] int pollId ,CancellationToken cancellationToken) {

            var result = await _resultService.GetPollVotesAsync(pollId,cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("votes-per-day")]
        public async Task<IActionResult> GetVotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken)
        {

            var result = await _resultService.GetVotesPerDayAsync(pollId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("votes-per-question")]
        public async Task<IActionResult> GetVotesPerQuestions([FromRoute] int pollId, CancellationToken cancellationToken)
        {

            var result = await _resultService.GetVotesPerQuestionsAsync(pollId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}
