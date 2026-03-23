using Microsoft.Extensions.Logging;
using Kyanendar.MVVM.Models;
using CommunityToolkit.Maui;
using Kyanendar.MVVM.ViewModels;
using Kyanendar.MVVM.Views;
using Microsoft.Maui.LifecycleEvents;

namespace Kyanendar
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureLifecycleEvents(events => {
#if ANDROID
                    events.AddAndroid(android => android
                        .OnStop((activity) => {
                            if (App.Current?.MainPage?.Handler?.MauiContext != null)
                            {
                                var currentPage = Shell.Current.CurrentPage;
                                if (currentPage is WeekView view)
                                {
                                    view.SaveState();
                                }
                            }
                        })
                        .OnDestroy((activity) => {
                            if (App.Current?.MainPage?.Handler?.MauiContext != null)
                            {
                                var currentPage = Shell.Current.CurrentPage;
                                if (currentPage is WeekView view)
                                {
                                    view.SaveState();
                                }
                            }
                        }));
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<KyanendarService>();
            builder.Services.AddSingleton<MenuViewModel>();
            builder.Services.AddSingleton<MenuView>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
