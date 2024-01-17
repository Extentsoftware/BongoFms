using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.CreateSprint
{
    public class CreateTaskCommand : IRequest<CreateTaskResponse>
    {
        public User User { get; set; } = default!;
        public Guid SprintId { get; set; }
        public SprintTaskCore Data { get; set; } = default!;
    }
}
