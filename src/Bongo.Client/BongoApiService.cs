using Bongo.Domain.Api;
using Flurl.Http;

namespace Bongo.Client
{
    public class BongoApiService(string baseUrl) : IBongoApiService
    {
        public async Task<GetSprintsResponse> GetSprintsAsync(CancellationToken cancellationToken)
        {
            var apiResult = await "{baseUrl}/api/sprints".GetJsonAsync<GetSprintsResponse>(cancellationToken);
            return apiResult;
        }
        
        public async Task<GetSprintResponse> GetSprintAsync(Guid id, CancellationToken cancellationToken)
        {
            var apiResult = await $"{baseUrl}/api/sprint/{id}".GetJsonAsync<GetSprintResponse>(cancellationToken);
            return apiResult;
        }
    }
}
