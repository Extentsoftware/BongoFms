using AutoMapper;
using BongoApplication.Handlers.GetSprints;
using BongoApplication.Handlers.SprintTaskUpdateAction;
using BongoDomain;
using BongoDomain.Api;

namespace BongoApplication.Mapping
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
