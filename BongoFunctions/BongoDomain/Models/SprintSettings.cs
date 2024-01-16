namespace BongoDomain
{
    public class SprintSettings
    {
        public string DefaultStateTemplateName { get; set; } = Constants.DefaultStateTemplate;
        public SprintStateTemplate[] SprintStateTemplates { get; set; } = default!;
        public string[] Locations { get; set; } = default!;
    }
}
