using Bongo.Domain.Api;

namespace Bongo.Client
{
    public interface IBongoApiService
    {
        Task<GetSprintsResponse> GetSprintsAsync(CancellationToken cancellationToken);
        Task<GetSprintResponse> GetSprintAsync(Guid id, CancellationToken cancellationToken);
        Task<SprintTaskUpdateActionResponse> SubmitActionsAsync(SprintTaskUpdateActionRequest request, CancellationToken cancellationToken);
        Task<CreateSprintResponse> CreateSprint(CreateSprintRequest request, CancellationToken cancellationToken);
    }
}
