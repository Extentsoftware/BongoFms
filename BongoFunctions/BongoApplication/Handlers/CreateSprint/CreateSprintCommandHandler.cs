using BongoApplication.Services.Elastic;
using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.CreateSprint
{
    public class CreateSprintCommandHandler(Nest.IElasticClient client, ElasticSprintConfiguration config) : IRequestHandler<CreateSprintCommand, CreateSprintResponse>
    {
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

            var settingsDocs = await client.SearchAsync<SprintSettings>(x => x.Index(config.SprintSettingsIndexName).Query(x => x.MatchAll()), cancellationToken);
            var settings = settingsDocs.Documents.First();
            var templateName = request.StateTemplateName ?? settings.DefaultStateTemplateName;
            var template = settings.SprintStateTemplates.First(x => x.Name == templateName);

            sprint.States = template.Items;

            await client.IndexAsync(sprint, x => x.Id(sprint.Id).Index(config.SprintIndexName), cancellationToken);
            return new CreateSprintResponse
            {
                IsSuccess = true,
                Sprint = sprint,
            };
        }
    }
}
