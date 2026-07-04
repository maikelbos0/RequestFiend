using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using RequestFiend.Core;
using RequestFiend.Models;
using Serilog;
using System.IO.Abstractions;
using System.Net.Http;
using System.Threading;

namespace RequestFiend.UI;

public static class MauiProgram {
    public static MauiApp CreateMauiApp()
        => MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .ConfigureServices()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FiraCode-Regular.ttf", "FiraCodeRegular");
            })
            .Build();

    private static MauiAppBuilder ConfigureServices(this MauiAppBuilder mauiAppBuilder) {
        mauiAppBuilder.Services.AddSingleton<IFileSystem, FileSystem>();
        mauiAppBuilder.Services.AddHttpClient<IExchangeHandler, ExchangeHandler>()
            .ConfigurePrimaryHttpMessageHandler(static serviceProvider => new SocketsHttpHandler() {
                PooledConnectionLifetime = System.TimeSpan.Zero,
                SslOptions = {
                    RemoteCertificateValidationCallback = serviceProvider.GetRequiredService<IServerCertificateValidationHandler>().Handle
                }
            })
            .ConfigureHttpClient(static httpClient => httpClient.Timeout = Timeout.InfiniteTimeSpan);
        mauiAppBuilder.Services.AddSingleton<IServerCertificateValidationHandler, ServerCertificateValidationHandler>();
        mauiAppBuilder.Services.AddSingleton<IScriptEvaluator, ScriptEvaluator>();
        mauiAppBuilder.Services.AddSingleton<IUserInterface, UserInterface>();

        mauiAppBuilder.Services.AddSingleton<Models.Services.IMessageService, Models.Services.MessageService>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IPopupService, Services.PopupService>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IPreferencesService, Models.Services.PreferencesService>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IModelDataProvider, Models.Services.ModelDataProvider>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IRequestTemplateCollectionService, Models.Services.RequestTemplateCollectionService>();
        mauiAppBuilder.Services.AddSingleton<Models.Services.IEnvironmentService, Models.Services.EnvironmentService>();

        mauiAppBuilder.Services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<Models.Services.IModelDataProvider>().GetData<FileModel>());
        mauiAppBuilder.Services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<Models.Services.IModelDataProvider>().GetData<RequestTemplateCollection>());
        mauiAppBuilder.Services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<Models.Services.IModelDataProvider>().GetData<RequestTemplate>());
        mauiAppBuilder.Services.AddTransient(serviceProvider => serviceProvider.GetRequiredService<Models.Services.IModelDataProvider>().GetData<Environment>());

        mauiAppBuilder.Services.AddSingleton<MainPageModel>();
        mauiAppBuilder.Services.AddSingleton<PreferencesModel>();
        mauiAppBuilder.Services.AddSingleton(serviceProvider => new ExchangeLogModel(
            serviceProvider.GetRequiredService<Models.Services.IMessageService>(),
            serviceProvider.GetRequiredService<Models.Services.IPopupService>(),
            1000
        ));
        mauiAppBuilder.Services.AddTransient<EnvironmentModel>();
        mauiAppBuilder.Services.AddTransient<RequestTemplateCollectionModel>();
        mauiAppBuilder.Services.AddTransient<ExchangeModel>();

        mauiAppBuilder.Services.AddSerilog((serviceProvider, loggerConfiguration) => {
            var preferencesService = serviceProvider.GetRequiredService<Models.Services.IPreferencesService>();
            var exchangeLoggingPath = preferencesService.GetExchangeLoggingPath();
            var exchangeLoggingOutputTemplate = preferencesService.GetExchangeLoggingOutputTemplate();

            loggerConfiguration.WriteTo.Sink(new Models.Services.ExchangeLogSink(serviceProvider.GetRequiredService<ExchangeLogModel>(), exchangeLoggingOutputTemplate));

            if (!string.IsNullOrWhiteSpace(exchangeLoggingPath)) {
                loggerConfiguration.WriteTo.File(exchangeLoggingPath, outputTemplate: exchangeLoggingOutputTemplate, rollingInterval: RollingInterval.Day);
            }
        });

        return mauiAppBuilder;
    }
}
