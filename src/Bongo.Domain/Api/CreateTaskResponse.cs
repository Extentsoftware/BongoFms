namespace BongoDomain.Api
{
    public class CreateTaskResponse : BaseResponse
    {
        public Sprint Sprint { get; set; } = default!;
        public Guid SprintTaskId { get; set; }

    }
}
