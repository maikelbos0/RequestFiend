using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.UI.Views;
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
        mauiAppBuilder.Services.AddSingleton<Models.Services.IPreferencesService, Models.Services.PreferencesService>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IPreferencesService, Models.Services.PreferencesService>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IModelDataProvider, Models.Services.ModelDataProvider>();

        mauiAppBuilder.Services.AddTransient<Models.Services.IRequestTemplateCollectionService, Models.Services.RequestTemplateCollectionService>();
        mauiAppBuilder.Services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<Models.Services.IModelDataProvider>().GetData<RequestTemplateCollectionFileModel>());
        mauiAppBuilder.Services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<Models.Services.IModelDataProvider>().GetData<RequestTemplateCollection>());
        mauiAppBuilder.Services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<Models.Services.IModelDataProvider>().GetData<RequestTemplate>());

        mauiAppBuilder.Services.AddTransient<MainPageModel>();
        mauiAppBuilder.Services.AddTransient<PreferencesModel>();
        mauiAppBuilder.Services.AddTransient<NewRequestTemplateModel>();
        mauiAppBuilder.Services.AddTransient<RequestTemplateCollectionSettingsModel>();
        mauiAppBuilder.Services.AddTransient<RequestTemplateModel>();

        mauiAppBuilder.Services.AddTransient<RequestTemplateCollectionSettingsPage>();
        mauiAppBuilder.Services.AddTransient<NewRequestTemplatePage>();
        mauiAppBuilder.Services.AddTransient<RequestTemplatePage>();

        return mauiAppBuilder;
    }
}

// TODO disable buttons when method not available
// TODO decide what the fuck to do with the JSON buttons
// TODO maybe just have an action button for creating a new request that adds a new empty request? Or move it to a popup?

