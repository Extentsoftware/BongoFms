using BongoApplication.Services.Elastic;
using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.GetSprints
{
    public class GetSprintsCommand : IRequest<GetSprintsResponse>
    {
        public User User { get; set; } = default!;
        public DateTime CreatedFrom { get; set; } = default!;
        public bool IncludeClosed { get; set; } = default!;
    }

    public class GetSprintsCommandHandler : IRequestHandler<GetSprintsCommand, GetSprintsResponse>
    {
        private readonly Nest.IElasticClient _client;
        private readonly ElasticSprintConfiguration _config;

        public GetSprintsCommandHandler(Nest.IElasticClient client, ElasticSprintConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<GetSprintsResponse> Handle(GetSprintsCommand request, CancellationToken cancellationToken)
        {
            var sprints = await _client.SearchAsync<SprintCore>(x => x.Index(_config.SprintIndexName).Query(x => x.MatchAll()), cancellationToken);

            return new GetSprintsResponse
            {
                IsSuccess = true,
                Sprints = [.. sprints.Documents]
            };
        }
    }
}
