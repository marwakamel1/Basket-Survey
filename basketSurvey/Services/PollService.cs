using basketSurvey.Models;

namespace basketSurvey.Services
{
    public class PollService : IPollService
    {
        private static readonly List<Poll> _polls = [
        new Poll
        {
            Id = 1,
            Title = "Poll 1",
            Description = "My first poll"
        }
        ];

        public Poll Add(Poll poll)
        {
            poll.Id = _polls.Count + 1;
            _polls.Add( poll );
            return poll;
        }

        public bool Delete(int id)
        {
            var poll = Get( id );
            if (poll is null) {
              return false;
            }

            _polls.Remove(poll);
            return true;
        }

        public Poll? Get(int id)
        {
            return _polls.SingleOrDefault(p => p.Id == id);
        }

        public IEnumerable<Poll> GetAll()
        {
            return _polls;
        }

        public bool Update(int id, Poll poll)
        {
            var foundpoll = Get(id);
            if(foundpoll is null)
                return false;
            foundpoll.Title = poll.Title;
            foundpoll.Description = poll.Description;
            return true;
            
        }
    }
}
