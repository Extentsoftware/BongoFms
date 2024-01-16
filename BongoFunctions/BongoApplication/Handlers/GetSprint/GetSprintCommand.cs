using BongoApplication.Services.Elastic;
using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.GetSprint
{
    public class GetSprintCommand : IRequest<GetSprintResponse>
    {
        public Guid SprintId { get; set; }
        public User User { get; set; } = default!;
    }

    public class GetSprintCommandHandler : IRequestHandler<GetSprintCommand, GetSprintResponse>
    {
        private readonly Nest.IElasticClient _client;
        private readonly ElasticSprintConfiguration _config;

        public GetSprintCommandHandler(Nest.IElasticClient client, ElasticSprintConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<GetSprintResponse> Handle(GetSprintCommand request, CancellationToken cancellationToken)
        {
            var sprint = await _client.GetAsync<Sprint>(request.SprintId, x => x.Index(_config.SprintIndexName), cancellationToken);

            return new GetSprintResponse
            {
                IsSuccess = sprint.IsValid,
                Sprint = sprint.IsValid ? sprint.Source : null
            };
        }
    }
}
