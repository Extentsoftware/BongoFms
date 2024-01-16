using BongoApplication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IConfiguration Configuration = default!;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(cfgbuilder =>
    {
        cfgbuilder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile("appsettings.Development.json", true, true);
        Configuration = cfgbuilder.Build();
    }
    )
    .ConfigureServices(services =>
    {
        services
                //.AddApplicationInsightsTelemetryWorkerService()
                //.ConfigureFunctionsApplicationInsights()
                .AddHttpClient()
                .AddLogging(loggerBuilder =>
                {
                    //loggerBuilder.ClearProviders();
                    //loggerBuilder.AddConsole();
                })
                .AddCustomMediator()
                .AddAutoMapper()
                .AddElasticSearch(Configuration);
    })
    .Build();

host.Run();
