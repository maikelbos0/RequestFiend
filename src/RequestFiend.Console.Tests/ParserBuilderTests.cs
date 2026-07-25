using NSubstitute;
using System.CommandLine;
using System.IO.Abstractions;
using System.Text.Json;
using Xunit;

namespace RequestFiend.Console.Tests;

public class ParsersTests {
    private sealed record Data(string Value);

    [Fact]
    public void BuildJsonFileParser_When_Valid() {
        var data = new Data("Foo");
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists("./Data.json").Returns(true);
        fileSystem.File.ReadAllText("./Data.json").Returns(JsonSerializer.Serialize(data));

        var subject = new ParserBuilder(fileSystem);

        var option = new Option<Data>("--data") {
            CustomParser = subject.BuildJsonFileParser<Data>("option '--data'"),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--data", "./Data.json"]);

        Assert.Empty(result.Errors);
        Assert.Equal(data, result.GetValue(option));
    }

    [Theory]
    [InlineData(true, "{\"Value\": \"Foo\"}", new string[] { }, "Missing required argument for option '--data'.")]
    [InlineData(true, "{\"Value\": \"Foo\"}", new string[] { "./Data.json", "./Data.json" }, "Received too many arguments for option '--data'.")]
    [InlineData(false, null, new string[] { "./Data.json" }, "Argument for option '--data' must be an existing file.")]
    [InlineData(true, "null", new string[] { "./Data.json" }, "Argument for option '--data' must be a valid JSON file.")]
    [InlineData(true, "Invalid", new string[] { "./Data.json" }, "Argument for option '--data' must be a valid JSON file: 'I' is an invalid start of a value. Path: $ | LineNumber: 0 | BytePositionInLine: 0.")]
    public void BuildJsonFileParser_When_Invalid(bool fileExists, string? fileContents, string[] arguments, string expectedError) {
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists("./Data.json").Returns(fileExists);
        fileSystem.File.ReadAllText("./Data.json").Returns(fileContents);

        var subject = new ParserBuilder(fileSystem);

        var option = new Option<Data>("--data") {
            CustomParser = subject.BuildJsonFileParser<Data>("option '--data'"),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--data", .. arguments]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("123", 123)]
    public void BuildSecondsParser_When_Valid(string argument, int expectedValue) {
        var subject = new ParserBuilder(Substitute.For<IFileSystem>());

        var option = new Option<int?>("--seconds") {
            CustomParser = subject.BuildSecondsParser("option '--seconds'"),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--seconds", argument]);

        Assert.Empty(result.Errors);
        Assert.Equal(expectedValue, result.GetValue(option));
    }

    [Theory]
    [InlineData(new string[] { }, "Missing required argument for option '--seconds'.")]
    [InlineData(new string[] { "1", "1" }, "Received too many arguments for option '--seconds'.")]
    [InlineData(new string[] { "" }, "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData(new string[] { "1a" }, "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData(new string[] { "0" }, "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData(new string[] { "-1" }, "Argument for option '--seconds' must be a positive number of seconds.")]
    public void BuildSecondsParser_When_Invalid(string[] arguments, string expectedError) {
        var subject = new ParserBuilder(Substitute.For<IFileSystem>());

        var option = new Option<int?>("--seconds") {
            CustomParser = subject.BuildSecondsParser("option '--seconds'"),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--seconds", .. arguments]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }
}
