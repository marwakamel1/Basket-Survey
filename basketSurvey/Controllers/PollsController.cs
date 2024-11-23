using Mapster;

namespace basketSurvey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController(IPollService pollService ) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;
    

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var polls = _pollService.GetAll();
            return Ok(polls.Adapt<IEnumerable<PollResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            var poll = _pollService.Get(id);
            return poll is null ? NotFound() : Ok(poll.Adapt<PollResponse>());
        }

        [HttpPost]
        public IActionResult Post([FromBody] PollRequest poll)
        {

            var newPoll = _pollService.Add(poll.Adapt<Poll>());

            return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll.Adapt<PollResponse>());

        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, PollRequest poll)
        {

            bool isUpdated = _pollService.Update(id, poll.Adapt<Poll>());
            return isUpdated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            bool isDeleted = _pollService.Delete(id);

            return isDeleted ? NoContent() : NotFound();
        }


    }
}
