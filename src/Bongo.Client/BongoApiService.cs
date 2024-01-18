using Bongo.Domain.Api;
using Flurl.Http;

namespace Bongo.Client
{
    public class BongoApiService(string baseUrl) : IBongoApiService
    {
        public Task<GetSprintsResponse> GetSprintsAsync(CancellationToken cancellationToken)
        => $"{baseUrl}/api/sprints".GetJsonAsync<GetSprintsResponse>();
        
        public Task<GetSprintResponse> GetSprintAsync(Guid id, CancellationToken cancellationToken)
        => $"{baseUrl}/api/sprint/{id}".GetJsonAsync<GetSprintResponse>(cancellationToken);
    }
}
