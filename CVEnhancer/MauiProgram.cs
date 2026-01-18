using CVEnhancer.Services;
using Microsoft.Extensions.Logging;

namespace CVEnhancer
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
                });
            
            // Services
            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<ExtractionService>();
            builder.Services.AddSingleton<MatchingService>();
            builder.Services.AddSingleton<AnalysisService>();

            // Shell
            builder.Services.AddSingleton<AppShell>();
            
            // Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DataPage>();
            builder.Services.AddTransient<LibraryPage>();
            builder.Services.AddTransient<GeneratePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
