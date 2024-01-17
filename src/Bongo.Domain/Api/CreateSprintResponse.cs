using Bongo.Domain.Models;

namespace Bongo.Domain.Api
{

    public class CreateSprintResponse : BaseResponse
    {
        public Sprint Sprint { get; set; } = default!;
    }
}
