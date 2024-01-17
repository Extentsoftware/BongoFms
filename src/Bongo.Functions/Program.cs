using Bongo.Application;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IConfiguration Configuration = default!;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(cfgbuilder =>
    {
    }
    )
    .ConfigureServices(services =>
    {
        var cfgbuilder = new ConfigurationBuilder();
        cfgbuilder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile("appsettings.Development.json", true, true);
        Configuration = cfgbuilder.Build();

        services
                .AddHttpClient()
                .AddLogging(loggerBuilder =>
                {
                })
                .AddCustomMediator()
                .AddAutoMapper()
                .AddElasticSearch(Configuration);
    })
    .Build();

host.Run();
