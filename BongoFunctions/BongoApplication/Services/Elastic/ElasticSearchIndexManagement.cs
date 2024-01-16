using BongoDomain;
using Microsoft.Extensions.Logging;
using Nest;
using Polly;
using Polly.Retry;
using Policy = Polly.Policy;

namespace BongoApplication.Services.Elastic
{
    public class ElasticSearchIndexManagement(ILogger<ElasticSearchIndexManagement> logger) : IElasticSearchIndexManagement
    {
        private const int Retries = 120;

        public async Task<bool> WaitForElasticSearchAsync(
            IElasticClient client,
            CancellationToken cancellationToken)
        {
            try
            {
                AsyncRetryPolicy retryPolicy = Policy
                           .Handle<Exception>()
                           .Or<ElasticClientResponseException>()
                           .WaitAndRetryAsync(
                               Retries,
                               attempt => TimeSpan.FromSeconds(1));

                await retryPolicy.ExecuteAsync(async () =>
                {
                    var result = await client.PingAsync(ct: cancellationToken);
                    if (result?.IsValid != true)
                    {
                        throw new ElasticClientResponseException("Something went wrong with the request");
                    }
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task DeleteIndexAsync(
            string name,
            IElasticClient client,
            CancellationToken cancellationToken = default)
        {
            var isIndexExists = (await client.Indices.ExistsAsync(name, ct: cancellationToken)).Exists;

            if (isIndexExists)
            {
                logger.LogInformation("----- Deleting index {IndexName}", name);
                await client.Indices.DeleteAsync(name, ct: cancellationToken);
            }
        }

        public async Task<IndexResult> SetAliasAsync(
            string aliasName,
            string indexName,
            IElasticClient client,
            CancellationToken cancellationToken = default)
        {
            // delete any index that has the same name as the alias
            // this scenario occurred because of a rogue integration test, causing the alias to fail to be applied
            // note: GetAsync will return indices and alaises with that name hence the extra check on the key name
            var idx = await client.Indices.GetAsync(aliasName, ct: cancellationToken);
            if (idx.IsValid && idx.Indices.Count == 1 && idx.Indices.First().Key == aliasName)
            {
                await client.Indices.DeleteAsync(aliasName, ct: cancellationToken);
            }

            if ((await client.Indices.AliasExistsAsync(aliasName, ct: cancellationToken)).Exists)
            {
                var oldAliasSettings = await client.Indices.GetAliasAsync(aliasName, ct: cancellationToken);

                if (!oldAliasSettings.IsValid)
                {
                    return new IndexResult
                    {
                        IsValid = false,
                        Exception = new InvalidOperationException($"Failed to retrieve existing alias mapping: {oldAliasSettings.DebugInformation}")
                    };
                }

                var result = await client.Indices.PutAliasAsync(indexName, aliasName, ct: cancellationToken);

                if (!result.IsValid)
                {
                    return new IndexResult
                    {
                        IsValid = false,
                        Exception = new InvalidOperationException($"Could not create alias {aliasName} to point to index {indexName}, {result.DebugInformation}")
                    };
                }

                foreach (var index in oldAliasSettings.Indices)
                {
                    logger.LogInformation("----- Found old index {IndexName}", index);
                    await DeleteIndexAsync(index.Key.Name, client, cancellationToken);
                }
            }
            else
            {
                var response = await client.Indices.PutAliasAsync(indexName, aliasName, ct: cancellationToken);

                if (!response.IsValid)
                {
                    return new IndexResult
                    {
                        IsValid = false,
                        Exception = new InvalidOperationException($"Could not set alias {aliasName} to point to index {indexName}, {response.DebugInformation}")
                    };
                }
            }

            return new IndexResult()
            {
                IsValid = true
            };
        }

        public bool IndexOrAliasExists(string indexName, IElasticClient client)
        {
            if ((client.Indices.AliasExists(indexName)).Exists)
            {
                logger.LogInformation("----- Index Alias {IndexName} exists", indexName);
                return true;
            }

            if ((client.Indices.Exists(indexName)).Exists)
            {
                logger.LogInformation("----- Index {IndexName} exists", indexName);
                return true;
            }

            return false;
        }

        public void EnsureIndiceExists(string sprintIndexName, string settingsIndexName, IElasticClient client)
        {
            SprintStateDefinition[] states = [
                new SprintStateDefinition { Id = Guid.NewGuid(), SprintState = "Todo" },
                new SprintStateDefinition { Id = Guid.NewGuid(), SprintState = "In Progress" },
                new SprintStateDefinition { Id = Guid.NewGuid(), SprintState = "Quality Check" },
                new SprintStateDefinition { Id = Guid.NewGuid(), SprintState = "Done" }
            ];

            if (!IndexOrAliasExists(settingsIndexName, client))
            {
                var settingresult = client.Indices.Create(settingsIndexName, c => c.Map<SprintSettings>(m => m.AutoMap<SprintSettings>()));
                if (!settingresult.IsValid)
                    throw new InvalidDataException("Failed to create index");

                var initial = new SprintSettings
                {
                    DefaultStateTemplateName = Constants.DefaultStateTemplate,
                    SprintStateTemplates = [
                        new SprintStateTemplate
                        {
                            Name = Constants.DefaultStateTemplate,
                            Items = states
                        }
                    ],
                    Locations = ["B1-S1", "B1-S2", "B2-S1A", "B2-S1B", "B2-S2", "B2-S3", "B2-S4", "B2-S5", "B2-S6", "B2-S7", "B2-S8", "Nursery", "Warehouse", "Farm"]
                };

                var indexResult = client.Index(initial, x => x.Index(settingsIndexName));
                if (!indexResult.IsValid)
                    throw new InvalidDataException("Failed to create initial settings");
            }

            if (!IndexOrAliasExists(sprintIndexName, client))
            {
                var sprintresult = client.Indices.Create(sprintIndexName, c => c.Map<Sprint>(m => m.AutoMap<Sprint>()));
                if (!sprintresult.IsValid)
                    throw new InvalidDataException("Failed to create index");
                var initialSprint = new Sprint
                {
                    Id = Guid.NewGuid(),
                    DateCreated = DateTime.UtcNow,
                    DateStarted = DateTime.UtcNow,
                    SprintName = "First sprint",
                    DisplayOrder = 0,
                    States = states,
                    Tasks = [
                        new SprintTask
                        {
                            Id = Guid.NewGuid(),
                            Notes = "no notes",
                            RecommendedTeamsSize = 5,
                            SprintTaskName = "Review new sprints",
                            StateId = states[0].Id,
                            ActualTeamSize = 5,
                            EstimatedDays = 4,
                            Location = "B1-S1",
                            Progress = 0,
                            History = [
                            ]
                        }
                     ]
                };
                var indexResult = client.Index(initialSprint, x => x.Index(sprintIndexName));
                if (!indexResult.IsValid)
                    throw new InvalidDataException("Failed to create initial sprint");
            }
        }
    }
}
