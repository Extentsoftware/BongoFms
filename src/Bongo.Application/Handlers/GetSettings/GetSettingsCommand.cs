using BongoApplication.Services.Elastic;
using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.GetSettings
{
    public class GetSettingsCommand : IRequest<GetSettingsResponse>
    {
    }

    public class GetSettingsCommandHandler(Nest.IElasticClient client, ElasticSprintConfiguration config) : IRequestHandler<GetSettingsCommand, GetSettingsResponse>
    {
        public async Task<GetSettingsResponse> Handle(GetSettingsCommand request, CancellationToken cancellationToken)
        {
            var settingsDocs = await client.SearchAsync<SprintSettings>(x => x.Index(config.SprintSettingsIndexName).Query(x => x.MatchAll()), cancellationToken);
            var settings = settingsDocs.Documents.FirstOrDefault();

            return new GetSettingsResponse
            {
                IsSuccess = settings!=null,
                SprintSettings = settings
            };
        }
    }
}
