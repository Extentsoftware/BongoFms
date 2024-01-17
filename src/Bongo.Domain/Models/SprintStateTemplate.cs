namespace BongoDomain
{
    public class SprintStateTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public SprintStateDefinition[] Items { get; set; } = default!;
    }
}
