using Bongo.Domain.Models;
namespace Bongo.Domain.Api
{
    public class GetSprintsResponse : BaseResponse
    {
        public List<SprintCoreId> Sprints { get; set; } = default!;
    }
}
