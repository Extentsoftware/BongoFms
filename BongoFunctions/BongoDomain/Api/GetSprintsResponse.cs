namespace BongoDomain.Api
{
    public class GetSprintsResponse : BaseResponse
    {
        public List<SprintCoreId> Sprints { get; set; } = default!;
    }
}
