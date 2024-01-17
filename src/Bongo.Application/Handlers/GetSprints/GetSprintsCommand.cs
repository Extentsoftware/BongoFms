using Bongo.Application.Services.Elastic;
using Bongo.Domain;
using Bongo.Domain.Api;
using Bongo.Domain.Models;
using MediatR;

namespace Bongo.Application.Handlers.GetSprints
{
    public class GetSprintsCommand : IRequest<GetSprintsResponse>
    {
        public User User { get; set; } = default!;
        public DateTime CreatedFrom { get; set; } = default!;
        public bool IncludeClosed { get; set; } = default!;
    }

    public class GetSprintsCommandHandler(Nest.IElasticClient client, ElasticSprintConfiguration config) : IRequestHandler<GetSprintsCommand, GetSprintsResponse>
    {
        public async Task<GetSprintsResponse> Handle(GetSprintsCommand request, CancellationToken cancellationToken)
        {
            var sprints = await client.SearchAsync<SprintCoreId>(x => x.Index(config.SprintIndexName).Query(x => x.MatchAll()), cancellationToken);

            return new GetSprintsResponse
            {
                IsSuccess = true,
                Sprints = [.. sprints.Documents]
            };
        }
    }
}
