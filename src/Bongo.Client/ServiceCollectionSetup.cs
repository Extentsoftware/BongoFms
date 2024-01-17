using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bongo.Client
{
    public class BongoApiConfiguration
    {
        public string BaseUrl { get; set; } = default!;
    }
    public static class ServiceCollectionSetup
    {
        public static IServiceCollection AddBongoApiService(this IServiceCollection services, IConfiguration configuration)
        {
            BongoApiConfiguration serviceConfiguration = new();
            configuration.GetSection(nameof(BongoApiConfiguration)).Bind(serviceConfiguration);
            services.AddSingleton(serviceConfiguration);
            return services.AddBongoApiService(serviceConfiguration.BaseUrl);
        }

        public static IServiceCollection AddBongoApiService(this IServiceCollection services, string baseUrl)
        {
            services.AddHttpClient();
            services.AddTransient((Func<IServiceProvider, IBongoApiService>)((IServiceProvider x) => new BongoApiService(baseUrl)));
            return services;
        }
    }
}
