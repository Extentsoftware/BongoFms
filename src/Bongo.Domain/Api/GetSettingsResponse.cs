using Bongo.Domain.Models;

namespace Bongo.Domain.Api
{
    public class GetSettingsResponse : BaseResponse
    {
        public SprintSettings? SprintSettings { get; set; }
    }
}
