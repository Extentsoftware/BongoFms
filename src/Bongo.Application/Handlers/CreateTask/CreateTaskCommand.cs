using Bongo.Domain;
using Bongo.Domain.Api;
using Bongo.Domain.Models;
using MediatR;

namespace Bongo.Application.Handlers.CreateSprint
{
    public class CreateTaskCommand : IRequest<CreateTaskResponse>
    {
        public User User { get; set; } = default!;
        public Guid SprintId { get; set; }
        public SprintTaskCore Data { get; set; } = default!;
    }
}
