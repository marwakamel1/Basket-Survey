using basketSurvey.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace basketSurvey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("")]
        public IActionResult GetAll()
        {
            return Ok(_pollService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            var poll = _pollService.Get(id);
            return poll is null ? NotFound() : Ok(poll);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Poll poll)
        {

            var newPoll = _pollService.Add(poll);

            return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);

        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Poll poll)
        {

            bool isUpdated = _pollService.Update(id, poll);
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
