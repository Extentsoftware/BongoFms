using Bongo.Application.Services.Elastic;
using Bongo.Domain;
using Bongo.Domain.Api;
using Bongo.Domain.Models;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bongo.Application.Handlers.SprintTaskUpdateAction
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
            int itemsChanged = 0;
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

                SprintTaskCore? update = action.Data;
                if (update == null)
                {
                    update = task;
                    update.StateId = action.StateId;
                }
                
                SprintTaskHistory sprintTaskHistory = new()
                {
                    Data = update,
                    DateCreated = request.SubmissionTime,
                    User = request.User,
                };

                task.History = [.. task.History.Append(sprintTaskHistory).OrderByDescending(x => x.DateCreated)];
                task.StateId = task.History[0].Data.StateId;

                await client.UpdateAsync<Sprint>(action.SprintId, x => x.Index(config.SprintIndexName), cancellationToken);
                ++itemsChanged;
            }
            return new SprintTaskUpdateActionResponse
            {
                IsSuccess = true,
                ItemsChanged = itemsChanged
            };
        }
    }
}
