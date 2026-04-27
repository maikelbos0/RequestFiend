using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using RequestFiend.Core;
using RequestFiend.Models;
using System.IO.Abstractions;
using System.Net.Http;

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
        mauiAppBuilder.Services.AddHttpClient<IRequestHandler, RequestHandler>()
            .ConfigurePrimaryHttpMessageHandler(static (serviceProvider) => new HttpClientHandler() {
                ServerCertificateCustomValidationCallback = serviceProvider.GetRequiredService<IServerCertificateValidationHandler>().Handle
            });
        mauiAppBuilder.Services.AddSingleton<IServerCertificateValidationHandler, ServerCertificateValidationHandler>();
        mauiAppBuilder.Services.AddSingleton<IScriptEvaluator, ScriptEvaluator>();

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
        mauiAppBuilder.Services.AddTransient<RequestTemplateCollectionModel>();
        mauiAppBuilder.Services.AddTransient<RequestModel>();

        return mauiAppBuilder;
    }
}
