using BongoApplication.Services.Elastic;
using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.SprintTaskUpdateAction
{
    public class SprintTaskUpdateActionCommand : IRequest<SprintTaskUpdateActionResponse>
    {
        public SprintTaskUpdate[] Actions { get; set; } = default!;
        public User User { get; set; } = default!;
        public DateTime SubmissionTime { get; set; }

    }

    public class SprintTaskUpdateActionCommandHandler(Nest.IElasticClient client, ElasticSprintConfiguration config) : IRequestHandler<SprintTaskUpdateActionCommand, SprintTaskUpdateActionResponse>
    {
        public async Task<SprintTaskUpdateActionResponse> Handle(SprintTaskUpdateActionCommand request, CancellationToken cancellationToken)
        {
            foreach (var action in request.Actions)
            {
                var getSprint = await client.GetAsync<Sprint>(action.SprintId, x => x.Index(config.SprintIndexName), cancellationToken);
                if (getSprint?.IsValid == false || getSprint?.Source == null)
                {
                    return new SprintTaskUpdateActionResponse
                    {
                        IsSuccess = false,
                        Message = $"Sprint {action.SprintId} does not exist"
                    };
                }
                var sprint = getSprint.Source;
                var task = Array.Find(sprint.Tasks, x => x.Id == action.SprintTaskId);
                if (task == null)
                {
                    return new SprintTaskUpdateActionResponse
                    {
                        IsSuccess = false,
                        Message = $"Task {action.SprintTaskId} does not exist in sprint {action.SprintId}"
                    };
                }

                SprintTaskHistory sprintTaskHistory = new()
                {
                    Data = action.Data,
                    DateCreated = request.SubmissionTime,
                    User = request.User,
                };

                task.History = [.. task.History.Append(sprintTaskHistory).OrderByDescending(x => x.DateCreated)];
                task.StateId = task.History[0].Data.StateId;

                await client.UpdateAsync<Sprint>(action.SprintId, x => x.Index(config.SprintIndexName), cancellationToken);
            }
            return new SprintTaskUpdateActionResponse
            {
                IsSuccess = true
            };
        }
    }
}
