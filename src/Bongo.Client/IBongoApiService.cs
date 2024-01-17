using Bongo.Domain.Api;

namespace Bongo.Client
{
    public interface IBongoApiService
    {
        Task<GetSprintsResponse> GetSprintsAsync(CancellationToken cancellationToken);
        Task<GetSprintResponse> GetSprintAsync(Guid id, CancellationToken cancellationToken);
    }
}
