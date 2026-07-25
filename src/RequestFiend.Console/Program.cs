using RequestFiend.Console;
using RequestFiend.Core;
using System.CommandLine;
using System.Threading.Tasks;

var collectionArgument = new Argument<RequestTemplateCollection>("collection") {
    Description = "Collection from which to execute requests",
    CustomParser = Parsers.CreateJsonFileParser<RequestTemplateCollection>("collection")
};
var allowScriptEvaluationOption = new Option<bool>("--allow-script-evaluation", "-s") {
    Description = "Enable the evaluation of configured request scripts"
};

var requestTimeoutInSecondsOption = new Option<System.TimeSpan?>("--request-timeout", "-t") {
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

rootCommand.SetAction((parseResult, cancellationToken) => {
    var collection = parseResult.GetRequiredValue(collectionArgument);
    var allowScriptEvaluation = parseResult.GetValue(allowScriptEvaluationOption);
    var requestTimeoutInSeconds = parseResult.GetValue(requestTimeoutInSecondsOption);
    var environment = parseResult.GetValue(environmentOption);

    System.Console.WriteLine(collection);
    System.Console.WriteLine(allowScriptEvaluation);
    System.Console.WriteLine(requestTimeoutInSeconds);
    System.Console.WriteLine(environment);

    return Task.CompletedTask;
});

await rootCommand.Parse(args).InvokeAsync();

// TODO add logging options
// TODO add filter
// TODO how to save results?
// TODO cross-platform?