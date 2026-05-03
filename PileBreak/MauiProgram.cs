using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PileBreak.Services;
using PileBreak.ViewModels;
using PileBreak.Views;

namespace PileBreak
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Servicesの登録
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<SteamApiService>();

            // ViewModelsの登録
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<AddMenuViewModel>();
            builder.Services.AddTransient<SettingViewModel>();
            builder.Services.AddTransient<HistoryViewModel>();

            // Viewsの登録
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<AddMenuPage>();
            builder.Services.AddTransient<SettingPage>();
            builder.Services.AddTransient<HistoryPage>();

            return builder.Build();
        }
    }
}
