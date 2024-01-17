using Nest;

namespace BongoApplication.Services.Elastic
{
    public interface IElasticSearchIndexManagement
    {
        Task DeleteIndexAsync(string name, IElasticClient client, CancellationToken cancellationToken = default);

        void EnsureIndiceExists(string sprintIndexName, string settingsIndexName, IElasticClient client);

        bool IndexOrAliasExists(string indexName, IElasticClient client);

        Task<IndexResult> SetAliasAsync(string aliasName, string indexName, IElasticClient client, CancellationToken cancellationToken = default);

        Task<bool> WaitForElasticSearchAsync(IElasticClient client, CancellationToken cancellationToken);
    }

}
