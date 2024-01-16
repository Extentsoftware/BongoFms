using AutoMapper;
using BongoApplication.Handlers.CreateSprint;
using BongoApplication.Services.Elastic;
using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.CreateTask
{
    public class CreateTaskCommandHandler(
        Nest.IElasticClient client,
        ElasticSprintConfiguration config,
        IMapper mapper) : IRequestHandler<CreateTaskCommand, CreateTaskResponse>
    {
        public async Task<CreateTaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var getSprint = await client.GetAsync<Sprint>(request.SprintId, x => x.Index(config.SprintIndexName), cancellationToken);
            if (getSprint?.IsValid == false || getSprint?.Source == null)
            {
                return new CreateTaskResponse
                {
                    IsSuccess = false,
                    Message = $"Sprint {request.SprintId} does not exist"
                };
            }
            var sprint = getSprint.Source;

            SprintTask task = mapper.Map<SprintTask>(request.Data);
            var orderedList = sprint
                .Tasks
                .Append(task)
                .OrderBy(x => x.Epic)
                .ThenBy(x => x.SprintTaskName);

            sprint.Tasks = [.. orderedList];

            await client.IndexAsync(sprint, x => x.Id(sprint.Id).Index(config.SprintIndexName), cancellationToken);
            return new CreateTaskResponse
            {
                IsSuccess = true,
                Sprint = sprint,
            };
        }
    }
}
