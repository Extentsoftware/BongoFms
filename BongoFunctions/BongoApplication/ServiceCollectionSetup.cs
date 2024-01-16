using AutoMapper;
using BongoApplication.Handlers.SprintTaskUpdateAction;
using BongoApplication.Mapping;
using BongoApplication.Services.Elastic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BongoApplication
{
    public static class ServiceCollectionSetup
    {
        [ExcludeFromCodeCoverage]
        public static IServiceCollection AddCustomMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(SprintTaskUpdateActionCommandHandler).GetTypeInfo().Assembly));
            return services;
        }


        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(AutoMapperProfile));
            });
            config.AssertConfigurationIsValid();
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }

        public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        IConfiguration configuration)
        {

            services.AddSingleton<IElasticSearchIndexManagement, ElasticSearchIndexManagement>();

            services.AddSingleton(
                _ =>
                {
                    var elasticSearchConfig = new ElasticSprintConfiguration();
                    configuration.GetSection(nameof(ElasticSprintConfiguration)).Bind(elasticSearchConfig);
                    return elasticSearchConfig;
                });


            services.AddSingleton<IElasticClient>(
                provider =>
                {
                    var cfg = provider.GetRequiredService<ElasticSprintConfiguration>();

                    var client = ElasticSearchClientFactory.CreateWithCredentials(
                        cfg.ElasticSearchUrl,
                        cfg.SprintIndexName,
                        cfg.SprintSettingsIndexName,
                        cfg.Username,
                        cfg.Password);

                    var elasticSearchIndexManagement = provider.GetRequiredService<IElasticSearchIndexManagement>();

                    elasticSearchIndexManagement.WaitForElasticSearchAsync(client, default).GetAwaiter().GetResult();

                    elasticSearchIndexManagement.DeleteIndexAsync(cfg.SprintIndexName, client).GetAwaiter().GetResult();
                    elasticSearchIndexManagement.DeleteIndexAsync(cfg.SprintSettingsIndexName, client).GetAwaiter().GetResult();
                    elasticSearchIndexManagement.EnsureIndiceExists(cfg.SprintIndexName, cfg.SprintSettingsIndexName, client);

                    return client;
                });

            return services;
        }


    }
}

