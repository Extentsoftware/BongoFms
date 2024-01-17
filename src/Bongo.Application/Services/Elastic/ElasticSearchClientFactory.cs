using Bongo.Domain;
using Bongo.Domain.Models;
using Nest;

namespace Bongo.Application.Services.Elastic
{
    public static class ElasticSearchClientFactory
    {
        public static ElasticClient CreateWithCredentials(
            string url,
            string sprintIndexName,
            string sprintSettingsIndexName,
            string? username = null,
            string? password = null)
        {
            var connection = new ConnectionSettings(new Uri(url))
                    .DefaultMappingFor<Sprint>(m => m.IndexName(sprintIndexName))
                    .DefaultMappingFor<SprintSettings>(m => m.IndexName(sprintSettingsIndexName))
                    .DefaultIndex(sprintIndexName);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                connection.BasicAuthentication(username, password);
            var client = new ElasticClient(connection);
            return client;
        }
    }
}
