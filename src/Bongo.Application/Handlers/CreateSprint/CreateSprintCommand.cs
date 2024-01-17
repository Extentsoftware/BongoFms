using Bongo.Domain;
using Bongo.Domain.Api;
using Bongo.Domain.Models;
using MediatR;

namespace Bongo.Application.Handlers.CreateSprint
{
    public class CreateSprintCommand : IRequest<CreateSprintResponse>
    {
        public User User { get; set; } = default!;
        public string SprintName { get; set; } = default!;
        public string? StateTemplateName { get; set; }
    }
}
