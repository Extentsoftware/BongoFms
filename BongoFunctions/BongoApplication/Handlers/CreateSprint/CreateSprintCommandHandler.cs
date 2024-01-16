using BongoApplication.Services.Elastic;
using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.CreateSprint
{
    public class CreateSprintCommandHandler : IRequestHandler<CreateSprintCommand, CreateSprintResponse>
    {
        private readonly Nest.IElasticClient _client;
        private readonly ElasticSprintConfiguration _config;

        public CreateSprintCommandHandler(Nest.IElasticClient client, ElasticSprintConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<CreateSprintResponse> Handle(CreateSprintCommand request, CancellationToken cancellationToken)
        {
            Sprint sprint = new()
            {
                DateCreated = DateTime.UtcNow,
                DateEnded = null,
                DateStarted = null,
                DisplayOrder = int.MaxValue,
                Id = Guid.NewGuid(),
                SprintName = request.SprintName,
                Tasks = []
            };

            var settingsDocs = await _client.SearchAsync<SprintSettings>(x => x.Index(_config.SprintSettingsIndexName).Query(x => x.MatchAll()), cancellationToken);
            var settings = settingsDocs.Documents.First();
            var templateName = request.StateTemplateName ?? settings.DefaultStateTemplateName;
            var template = settings.SprintStateTemplates.First(x => x.Name == templateName);

            sprint.States = template.Items;

            await _client.IndexAsync(sprint, x => x.Id(sprint.Id).Index(_config.SprintIndexName), cancellationToken);
            return new CreateSprintResponse
            {
                IsSuccess = true,
                Sprint = sprint,
            };
        }
    }
}
