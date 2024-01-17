using BongoDomain;
using BongoDomain.Api;
using MediatR;

namespace BongoApplication.Handlers.CreateSprint
{
    public class CreateSprintCommand : IRequest<CreateSprintResponse>
    {
        public User User { get; set; } = default!;
        public string SprintName { get; set; } = default!;
        public string? StateTemplateName { get; set; }
    }
}
