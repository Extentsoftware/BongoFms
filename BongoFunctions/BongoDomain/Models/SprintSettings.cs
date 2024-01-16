namespace BongoDomain
{
    public class SprintSettings
    {
        public string DefaultStateTemplateName { get; set; } = Constants.DefaultStateTemplate;
        public SprintStateTemplate[] SprintStateTemplates { get; set; } = default!;
        public string[] Locations { get; set; } = default!;
        public string[] Tasks { get; set; } = default!;
        public Material[] Materials { get; set; } = default!;
    }
}
