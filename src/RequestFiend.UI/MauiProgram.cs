using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using RequestFiend.Models;
using System.IO.Abstractions;

namespace RequestFiend.UI;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureServices()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FiraCode-Regular.ttf", "FiraCodeRegular");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static MauiAppBuilder ConfigureServices(this MauiAppBuilder mauiAppBuilder) {
        mauiAppBuilder.Services.AddSingleton<IFileSystem, FileSystem>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IMessageService, Models.Services.MessageService>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IPopupService, Services.PopupService>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IRecentCollectionService, Services.RecentCollectionService>();
        mauiAppBuilder.Services.AddSingleton(typeof(Models.Services.IModelDataProvider<>), typeof(Models.Services.ModelDataProvider<>));

        mauiAppBuilder.Services.AddTransient<Models.Services.IRequestTemplateCollectionService, Models.Services.RequestTemplateCollectionService>();
        mauiAppBuilder.Services.AddTransient<MainPageModel>();
        mauiAppBuilder.Services.AddTransient<NewRequestTemplateModel>();
        mauiAppBuilder.Services.AddTransient<RequestTemplateCollectionModel>();
        mauiAppBuilder.Services.AddTransient<RequestTemplateModel>();

        return mauiAppBuilder;
    }
}

// TODO register pages in service collection
// TODO structure page/shell item titles
