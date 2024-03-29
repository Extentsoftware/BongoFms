﻿using Bongo.Client;
using Bongo.Fms.Services;
using Bongo.Fms.Views;
using Microsoft.Extensions.Logging;

namespace Bongo.Fms
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa_solid.ttf", "FontAwesome");
                });

            string defaultApiKey = Environment.GetEnvironmentVariable("bfms_apikey") ?? "somekey";
            string defaultbBaseUrl = Environment.GetEnvironmentVariable("bfms_baseurl") ?? "http://iot.vestrong.eu:8080";

            var baseUrl = Preferences.Get("ApiUrl", defaultbBaseUrl);
            string apiKey = Preferences.Get("ApiKey", defaultApiKey);
            
            builder.Services.AddBongoApiService(baseUrl, apiKey);
            builder.Services.AddTransient<ICachedDataService, CachedDataService>();
            builder.Services.AddTransient<StockListPage>();
            builder.Services.AddTransient<SprintListPage>();
            builder.Services.AddTransient<ObservationsPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<SprintTasksPage>();

            Routing.RegisterRoute("sprint/tasks", typeof(SprintTasksPage));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
