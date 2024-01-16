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
                    Locations = ["B1-S1", "B1-S2", "B2-S1A", "B2-S1B", "B2-S2", "B2-S3", "B2-S4", "B2-S5", "B2-S6", "B2-S7", "B2-S8", "Nursery", "Warehouse", "Farm"],
                    Tasks = ["Clearing", "Construction", "Fertilising", "Fireguard", "Groundwork", "Harvesting", "Irrigation - Bowser", "Irrigation", "Liming", "Maintenance", "Mowing", "Mulching", "Nursery", "Other", "Pegging", "Planting", "Pruning", "Scouting", "Security", "Slashing", "Spraying", "Stacking", "Weeding"],
                    Materials = [
                        new Material(Guid.NewGuid(), "AN", "Fertiliser", "Nitrate (N)", "Ammonium Nitrate"),
                        new Material(Guid.NewGuid(), "Compound J", "Fertiliser", "Mix", ""),
                        new Material(Guid.NewGuid(), "Cow Manure", "Fertiliser", "Mix", ""),
                        new Material(Guid.NewGuid(), "Foliar Fert", "Fertiliser", "Mix", ""),
                        new Material(Guid.NewGuid(), "Grow", "Fertiliser", "", ""),
                        new Material(Guid.NewGuid(), "Husks", "Fertiliser", "", ""),
                        new Material(Guid.NewGuid(), "Kelp-P-Max", "Fertiliser", "Mix", ""),
                        new Material(Guid.NewGuid(), "Lime (Dolomite)", "Fertiliser", "Lime", "Soil conditioning"),
                        new Material(Guid.NewGuid(), "MAP", "Fertiliser", "Phosphate (P)", "Mono Ammonium Phospate"),
                        new Material(Guid.NewGuid(), "MOP", "Fertiliser", "Potasium (K)", "Murate of Potash"),
                        new Material(Guid.NewGuid(), "SSP", "Fertiliser", "Phosphate (P)", "Super Single Phosphate"),
                        new Material(Guid.NewGuid(), "Urea", "Fertiliser", "Nitrate (N)", ""),
                        new Material(Guid.NewGuid(), "Omniboost", "Foliar feed", "Mix", ""),
                        new Material(Guid.NewGuid(), "Alliete", "Fungicide", "", "Phytophthora, downy mildew, canker."),
                        new Material(Guid.NewGuid(), "Chlorothalonil", "Fungicide", "!", "Banned in EU"),
                        new Material(Guid.NewGuid(), "Copper oxychloride", "Fungicide", "", "Scab, Anthracnose, Anthracnose, Downy mildew, Early blight, and Late blight"),
                        new Material(Guid.NewGuid(), "Curethane (Mancozeb)", "Fungicide", "", "broad spectrum"),
                        new Material(Guid.NewGuid(), "Dithane M45", "Fungicide", "", "broad spectrum - Leafspots, rust, botrytis, anthracnose, early and late blights, and downy mildew."),
                        new Material(Guid.NewGuid(), "Fosetyl", "Fungicide", "", "Phytophthora, Pythium and Plasmopara"),
                        new Material(Guid.NewGuid(), "Metalaxyl", "Fungicide", "", "possibly ineffective - Pythium"),
                        new Material(Guid.NewGuid(), "Gibberellic acid", "Hormone", "", "Hormone for root growth"),
                        new Material(Guid.NewGuid(), "Seradix", "Hormone", "", "Rooting compound"),
                        new Material(Guid.NewGuid(), "Acephate", "Insecticide", "", "aphids, leaf miners, caterpillars, sawflies, thrips,"),
                        new Material(Guid.NewGuid(), "Acetacure", "Insecticide", "", "thrips, whiteflies and aphids"),
                        new Material(Guid.NewGuid(), "Acol Lambda", "Insecticide", "!", "stink bug - not more often than 82 days"),
                        new Material(Guid.NewGuid(), "Actellic Gold Dust", "Insecticide", "", "Used to treat grain before bagging"),
                        new Material(Guid.NewGuid(), "Diazimone (Diazinon)", "Insecticide", "", "broad spectrum - Potato blight; Leaf spot; Scab; Rust"),
                        new Material(Guid.NewGuid(), "Dichlorvos", "Insecticide", "", "indoor use mushroom flies, aphids, spider mites, caterpillars, thrips, and whiteflies"),
                        new Material(Guid.NewGuid(), "Fencure (Fenvarelate)", "Insecticide", "", "Insecticide (toxic to bees)"),
                        new Material(Guid.NewGuid(), "Malathion", "Insecticide", "", "mosquitoes, aphids, whiteflies, mealybugs, red spider mites and scales"),
                        new Material(Guid.NewGuid(), "Thunder", "Insecticide", "", "use for Thrips (insect)"),
                        new Material(Guid.NewGuid(), "Carbofuran", "Pesticide", "!", "green leafhoppers, brown plant hoppers, stem borers and whorl maggots"),
                        new Material(Guid.NewGuid(), "Chlorpyrifos", "Pesticide", "!", "control of foliage and soil-born insects"),
                        new Material(Guid.NewGuid(), "Glyphosate", "Weed killer", "", ""),
                        new Material(Guid.NewGuid(), "Agriwett", "Wetter", "", "non-ionic wetter, makes spraying easier"),
                        new Material(Guid.NewGuid(), "Alvarion ", "Fungicide", "", ""),
                        new Material(Guid.NewGuid(), "Zinc Oxide", "Micronutrient", "", ""),
                        new Material(Guid.NewGuid(), "Calcium Nitrate", "Fertiliser", "Nitrate (N)", ""),
                        new Material(Guid.NewGuid(), "Carbaryl", "Pesticide", "", ""),
                        new Material(Guid.NewGuid(), "Acomil", "Fungicide", "", ""),
                        new Material(Guid.NewGuid(), "Boron", "Micronutrient", "", ""),
                        new Material(Guid.NewGuid(), "Etidot-67", "Boron", "", ""),
                        new Material(Guid.NewGuid(), "Compost ", "Fertiliser", "Mix", ""),
                        new Material(Guid.NewGuid(), "Cyhalothrin", "Pesticide", "", ""),
                        new Material(Guid.NewGuid(), "Bellis", "Fungicide", "", "")
                    ]
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
                            SprintTaskName = "Weeding",
                            StateId = states[0].Id,
                            ActualTeamSize = 5,
                            EstimatedDays = 4,
                            Location = "B1-S1",
                            Progress = 0, 
                            Epic="Crew A", 
                            DateCreated = DateTime.UtcNow,
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
