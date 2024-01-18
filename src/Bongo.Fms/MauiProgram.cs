using Bongo.Client;
using Bongo.Fms.Services;
using Bongo.Fms.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

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

            var baseUrl = Preferences.Get("ApiUrl", "http://localhost:7020");
            builder.Services.AddBongoApiService(baseUrl);
            builder.Services.AddTransient<ICachedDataService, CachedDataService>();
            builder.Services.AddTransient<StockListPage>();
            builder.Services.AddTransient<SprintListPage>();
            builder.Services.AddTransient<ObservationsPage>();
            builder.Services.AddTransient<MainPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
