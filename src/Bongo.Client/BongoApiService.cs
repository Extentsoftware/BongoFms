using Bongo.Domain.Api;
using Flurl.Http;

namespace Bongo.Client
{
    public class BongoApiService(string baseUrl, string apiKey) : IBongoApiService
    {
        private const string FunctionsKey = "x-functions-key";

        public Task<GetSprintsResponse> GetSprintsAsync(CancellationToken cancellationToken)
        => $"{baseUrl}/api/sprints"
            .WithHeader(FunctionsKey, apiKey)
            .GetJsonAsync<GetSprintsResponse>();
        
        public Task<GetSprintResponse> GetSprintAsync(Guid id, CancellationToken cancellationToken)
        => $"{baseUrl}/api/sprint/{id}"
            .WithHeader(FunctionsKey, apiKey)
            .GetJsonAsync<GetSprintResponse>(cancellationToken);

        public Task<SprintTaskUpdateActionResponse> SubmitActionsAsync(SprintTaskUpdateActionRequest request, CancellationToken cancellationToken)
        => $"{baseUrl}/api/submitactions"
                .WithHeader(FunctionsKey, apiKey)
                .PostJsonAsync(request, cancellationToken)
                .ReceiveJson<SprintTaskUpdateActionResponse>();

        public Task<CreateSprintResponse> CreateSprint(CreateSprintRequest request, CancellationToken cancellationToken)
        => $"{baseUrl}/api/sprint"
                .WithHeader(FunctionsKey, apiKey)
                .PostJsonAsync(request, cancellationToken)
                .ReceiveJson<CreateSprintResponse>();
    }
}
