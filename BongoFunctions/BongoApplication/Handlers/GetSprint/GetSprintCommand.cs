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

    public class GetSprintCommandHandler(Nest.IElasticClient client, ElasticSprintConfiguration config) : IRequestHandler<GetSprintCommand, GetSprintResponse>
    {
        public async Task<GetSprintResponse> Handle(GetSprintCommand request, CancellationToken cancellationToken)
        {
            var sprint = await client.GetAsync<Sprint>(request.SprintId, x => x.Index(config.SprintIndexName), cancellationToken);

            return new GetSprintResponse
            {
                IsSuccess = sprint.IsValid,
                Sprint = sprint.IsValid ? sprint.Source : null
            };
        }
    }
}
