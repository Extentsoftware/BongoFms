using AutoMapper;
using Bongo.Application.Handlers.GetSprints;
using Bongo.Application.Handlers.SprintTaskUpdateAction;
using Bongo.Domain;
using Bongo.Domain.Api;
using Bongo.Domain.Models;

namespace Bongo.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SprintTaskUpdateActionCommand, SprintTaskUpdateActionRequest>().ReverseMap();
            CreateMap<GetSprintsCommand, GetSprintsRequest>().ReverseMap();
            CreateMap<SprintTaskCore, SprintTaskCoreId>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ReverseMap();

            CreateMap<SprintTaskCoreId, SprintTask>()
                .ForMember(x => x.History, x => x.Ignore())
                .ReverseMap();

        }
    }
}
