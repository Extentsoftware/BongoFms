using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bongo.Client
{
    public class BongoApiConfiguration
    {
        public string BaseUrl { get; set; } = default!;
        public string ApiKeySecret { get; set; } = default!;
    }
    public static class ServiceCollectionSetup
    {
        public static IServiceCollection AddBongoApiService(this IServiceCollection services, string baseUrl, string apiKey)
        {
            services.AddHttpClient();
            
            services.AddTransient<IBongoApiService>(x =>
            {
                return new BongoApiService(baseUrl, apiKey);
            });
            return services;
        }
    }
}
