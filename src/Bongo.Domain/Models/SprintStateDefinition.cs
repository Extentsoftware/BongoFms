namespace Bongo.Domain.Models
{
    public class SprintStateDefinition
    {
        public Guid Id { get; set; }
        public string SprintState { get; set; } = default!;
    }
}
