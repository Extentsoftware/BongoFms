using Bongo.Domain.Models;
namespace Bongo.Domain.Api
{

    public class SprintTaskUpdateActionRequest
    {
        public SprintTaskUpdate[] Actions { get; set; } = default!;
        public User User { get; set; } = default!;
        public DateTime SubmissionTime { get; set; }
    }

    public class SprintTaskUpdate
    {
        public Guid SprintId { get; set; }
        public Guid SprintTaskId { get; set; }
        
        // either set StateId OR Data. If both are set then Data is taken
        public Guid StateId { get; set; }
        public SprintTaskCore Data { get; set; } = default!;
    }
}
