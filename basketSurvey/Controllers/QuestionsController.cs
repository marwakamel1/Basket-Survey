using basketSurvey.Abstractions;
using basketSurvey.Contracts.Polls;
using basketSurvey.Contracts.Questions;
using basketSurvey.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace basketSurvey.Controllers
{
    [Route("api/Polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        public IQuestionService _questionService  = questionService;


        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id ,CancellationToken cancellationToken)
        {
            var result = await _questionService.GetAsync(pollId,id,cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int pollId,CancellationToken cancellationToken)
        {
            var result = await _questionService.GetAllAsync(pollId,cancellationToken);

           return result.IsSuccess ? Ok(result.Value):result.ToProblem();
           
        }

     

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {

            var result = await _questionService.AddAsync(pollId, request, cancellationToken);

            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { pollId , id = result.Value.Id }, result.Value)
                : result.ToProblem();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int pollId, [FromRoute] int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {

            var result = await _questionService.UpdateAsync(pollId, id, request, cancellationToken);
            return result.IsSuccess ? Ok()
                  : result.ToProblem();
            //: Problem(statusCode: StatusCodes.Status404NotFound, title : result.Error.code ,detail :result.Error.description);
        }

        [HttpPut("{id}/toggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();

        }

     

    }
}
