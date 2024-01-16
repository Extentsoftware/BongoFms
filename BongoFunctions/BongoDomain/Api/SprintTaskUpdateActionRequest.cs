namespace BongoDomain.Api
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
        public SprintTaskCore Data { get; set; } = default!;
    }

}
