using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;

var collectionArgument = new Argument<FileInfo>("collection") {
    Description = "Collection from which to execute requests"
};
var rootCommand = new RootCommand("RequestFiend - An open source platform for managing and executing API requests.");
rootCommand.Arguments.Add(collectionArgument);
rootCommand.SetAction((parseResult, cancellationToken) => {
    var collectionFile = parseResult.GetValue(collectionArgument) ?? throw new InvalidOperationException();

    Console.WriteLine(collectionFile.FullName);

    return Task.CompletedTask;
});

await rootCommand.Parse(args).InvokeAsync();
