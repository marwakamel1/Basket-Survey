using Microsoft.AspNetCore.Authorization;

namespace basketSurvey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollsController(IPollService pollService ) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;
    

        [HttpGet("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _pollService.GetAllAsync(cancellationToken);
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id , CancellationToken cancellationToken)
        {

            var result = await _pollService.GetAsync(id , cancellationToken);
            return result.IsSuccess ? Ok(result.Value) :
                result.ToProblem();
                //Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.code, detail: result.Error.description);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PollRequest poll, CancellationToken cancellationToken )
        {

            var result = await _pollService.AddAsync(poll, cancellationToken);

            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
            

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PollRequest poll, CancellationToken cancellationToken)
        {

            var result = await _pollService.UpdateAsync(id, poll, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
                //: Problem(statusCode: StatusCodes.Status404NotFound, title : result.Error.code ,detail :result.Error.description);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {

            var result = await _pollService.DeleteAsync(id, cancellationToken);

            return result.IsSuccess ? NoContent()
                  : result.ToProblem();
            //: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.code, detail: result.Error.description);
        }

        [HttpPut("{id}/togglePublish")]
        public async Task<IActionResult> togglePublish(int id, CancellationToken cancellationToken)
        {

            var result = await _pollService.TogglePublishStatusAsync(id,  cancellationToken);
            return result.IsSuccess ? NoContent()
                  : result.ToProblem();
            //: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.code, detail: result.Error.description);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var result = await _pollService.GetCurrentAsync(cancellationToken);
            return Ok(result.Value);
        }

    }
}
