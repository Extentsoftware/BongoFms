using Bongo.Domain.Models;
namespace Bongo.Domain.Api
{
    public class GetSprintResponse : BaseResponse
    {
        public Sprint? Sprint { get; set; }
    }
}
