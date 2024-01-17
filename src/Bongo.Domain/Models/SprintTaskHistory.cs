namespace Bongo.Domain.Models
{
    public class SprintTaskHistory
    {
        public SprintTaskCore Data { get; set; } = default!;
        public DateTime DateCreated { get; set; }
        public User User { get; set; } = default!;
    }
}
