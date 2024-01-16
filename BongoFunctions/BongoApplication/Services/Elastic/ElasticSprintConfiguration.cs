namespace BongoApplication.Services.Elastic
{
    public class ElasticSprintConfiguration
    {
        public string SprintIndexName { get; set; } = default!;
        public string SprintSettingsIndexName { get; set; } = default!;
        public string ElasticSearchUrl { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;

    }

}
