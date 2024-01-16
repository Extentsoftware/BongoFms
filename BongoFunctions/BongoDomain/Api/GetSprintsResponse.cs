namespace BongoDomain.Api
{
    public class GetSprintsResponse : BaseResponse
    {
        public List<SprintCore> Sprints { get; set; } = default!;
    }
}
