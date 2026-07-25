using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestFiend.Console;
using RequestFiend.Core;
using System.CommandLine;
using System.Net.Http;
using System.Threading;

var builder = Host.CreateApplicationBuilder(args);

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
builder.Services.AddSingleton<ICommandHandler, CommandHandler>();

var host = builder.Build();

var collectionArgument = new Argument<RequestTemplateCollection>("collection") {
    Description = "Collection from which to execute requests",
    CustomParser = Parsers.CreateJsonFileParser<RequestTemplateCollection>("collection")
};
var allowScriptEvaluationOption = new Option<bool>("--allow-script-evaluation", "-s") {
    Description = "Enable the evaluation of configured request scripts"
};

var requestTimeoutInSecondsOption = new Option<int?>("--request-timeout", "-t") {
    Description = "Timeout in seconds for executing requests",
    CustomParser = Parsers.CreateSecondsParser("option '--request-timeout'"),
    Arity = ArgumentArity.ZeroOrMore,
    AllowMultipleArgumentsPerToken = true
};
var environmentOption = new Option<Environment?>("--environment", "-e") {
    Description = "Environment from which to use variables",
    CustomParser = Parsers.CreateJsonFileParser<Environment>("option '--environment'"),
    Arity = ArgumentArity.ZeroOrMore,
    AllowMultipleArgumentsPerToken = true
};
var rootCommand = new RootCommand("RequestFiend - An open source platform for managing and executing API requests.") {
    collectionArgument,
    allowScriptEvaluationOption,
    requestTimeoutInSecondsOption,
    environmentOption
};

rootCommand.SetAction(async (parseResult, cancellationToken) => {
    var collection = parseResult.GetRequiredValue(collectionArgument);
    var options = new ExchangeOptions(parseResult.GetValue(allowScriptEvaluationOption), parseResult.GetValue(requestTimeoutInSecondsOption));
    var environment = parseResult.GetValue(environmentOption);
    var handler = host.Services.GetRequiredService<ICommandHandler>();

    await handler.ExecuteRequests(collection, options, environment, cancellationToken);
});

await rootCommand.Parse(args).InvokeAsync();

// TODO add logging options
// TODO add filter
// TODO how to save results?
// TODO cross-platform?