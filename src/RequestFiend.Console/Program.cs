using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestFiend.Console;
using RequestFiend.Core;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using System.CommandLine;
using System.IO.Abstractions;
using System.Net.Http;
using System.Threading;

var builder = Host.CreateApplicationBuilder();

builder.Configuration.AddCommandLine(args, new Dictionary<string, string>() {
    { "--logging-path", "logging-path" },
    { "-lp", "logging-path" },
    { "--logging-output-template", "logging-output-template" },
    { "-lo", "logging-output-template" },
    { "--minimum-exchange-logging-level", "minimum-exchange-logging-level" },
    { "-me", "minimum-exchange-logging-level" },
    { "--minimum-other-source-logging-level", "minimum-other-source-logging-level" },
    { "-mo", "minimum-other-source-logging-level" },
});
builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddSingleton<ParserBuilder>();
builder.Services.AddHttpClient<IExchangeHandler, ExchangeHandler>()
    .ConfigurePrimaryHttpMessageHandler(static serviceProvider => new SocketsHttpHandler() {
        PooledConnectionLifetime = System.TimeSpan.Zero,
        SslOptions = {
            RemoteCertificateValidationCallback = serviceProvider.GetRequiredService<IServerCertificateValidationHandler>().Handle
        }
    })
    .ConfigureHttpClient(static httpClient => httpClient.Timeout = Timeout.InfiniteTimeSpan);
builder.Services.AddSingleton<IServerCertificateValidationHandler, ServerCertificateValidationHandler>();
builder.Services.AddSingleton<IScriptEvaluator, ScriptEvaluator>();
builder.Services.AddSingleton<CommandHandler>();

builder.Services.AddSerilog((serviceProvider, loggerConfiguration) => {
    var loggingPath = builder.Configuration["logging-path"];
    var loggingOutputTemplate = builder.Configuration["logging-output-template"] ?? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    if (System.Enum.TryParse<LogEventLevel>(builder.Configuration["minimum-other-source-logging-level"], out var minimumOtherSourceLoggingLevel)) {
        loggerConfiguration.MinimumLevel.Override(nameof(RequestFiend), minimumOtherSourceLoggingLevel);
    }

    if (System.Enum.TryParse<LogEventLevel>(builder.Configuration["minimum-exchange-logging-level"], out var minimumExchangeLoggingLevel)) {
        loggerConfiguration.MinimumLevel.Override(nameof(RequestFiend), minimumExchangeLoggingLevel);
    }

    loggerConfiguration.WriteTo.Console(outputTemplate: loggingOutputTemplate);

    if (!string.IsNullOrWhiteSpace(loggingPath)) {
        loggerConfiguration.WriteTo.File(loggingPath, outputTemplate: loggingOutputTemplate, rollingInterval: RollingInterval.Day);
    }
});

var host = builder.Build();

ContentManagerProvider.Initialize(host.Services.GetRequiredService<IFileSystem>());

var parserBuilder = host.Services.GetRequiredService<ParserBuilder>();

var collectionArgument = new Argument<RequestTemplateCollection>("collection") {
    Description = "Collection from which to execute requests",
    CustomParser = parserBuilder.BuildJsonFileParser<RequestTemplateCollection>("collection")
};
var allowScriptEvaluationOption = new Option<bool>("--allow-script-evaluation", "-s") {
    Description = "Enable the evaluation of configured request scripts"
};

var requestTimeoutInSecondsOption = new Option<int?>("--request-timeout", "-t") {
    Description = "Timeout in seconds for executing requests",
    CustomParser = parserBuilder.BuildSecondsParser("option '--request-timeout'"),
    Arity = ArgumentArity.ZeroOrMore,
    AllowMultipleArgumentsPerToken = true
};
var environmentOption = new Option<Environment?>("--environment", "-e") {
    Description = "Environment from which to use variables",
    CustomParser = parserBuilder.BuildJsonFileParser<Environment>("option '--environment'"),
    Arity = ArgumentArity.ZeroOrMore,
    AllowMultipleArgumentsPerToken = true
};
var rootCommand = new RootCommand("RequestFiend - An open source platform for managing and executing API requests.") {
    collectionArgument,
    allowScriptEvaluationOption,
    requestTimeoutInSecondsOption,
    environmentOption,
    new Option<string>("--logging-path", "-lp") {
        Description = "File path for logging",
        CustomParser = _ => null,
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true
    },
    new Option<string>("--logging-output-template", "-lo") {
        Description = "Logging output template (Serilog style",
        CustomParser = _ => null,
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true
    },
    new Option<string>("--minimum-exchange-logging-level", "-me") {
        Description = "Minimum level required for logging from request execution",
        CustomParser = _ => null,
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true
    },
    new Option<string>("--minimum-other-source-logging-level", "-mo") {
        Description = "Minimum level required for logging from other sources",
        CustomParser = _ => null,
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true
    }
};

rootCommand.SetAction(async (parseResult, cancellationToken) => {
    var collection = parseResult.GetRequiredValue(collectionArgument);
    var options = new ExchangeOptions(parseResult.GetValue(allowScriptEvaluationOption), parseResult.GetValue(requestTimeoutInSecondsOption));
    var environment = parseResult.GetValue(environmentOption);
    var handler = host.Services.GetRequiredService<CommandHandler>();

    await handler.ExecuteRequests(collection, options, environment, cancellationToken);
});

await rootCommand.Parse(args).InvokeAsync();

// TODO add filter
// TODO how to save results?
// TODO cross-platform?