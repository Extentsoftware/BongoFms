namespace BongoDomain
{
    public class SprintTaskCore
    {
        public string SprintTaskName { get; set; } = default!;
        public string Notes { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string Epic { get; set; } = default!;
        public int RecommendedTeamsSize { get; set; }
        public int ActualTeamSize { get; set; }
        public int EstimatedDays { get; set; }
        public int Progress { get; set; }
        public Guid StateId { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class SprintTaskCoreId : SprintTaskCore
    {
        public Guid Id { get; set; }
    }

    public class SprintTask : SprintTaskCoreId
    {

        public SprintTaskHistory[] History { get; set; } = default!;
    }
}
