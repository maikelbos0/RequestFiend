using System;
using System.CommandLine;
using Xunit;

namespace RequestFiend.Console.Tests;

public class ParsersTests {
    private sealed record Data(string Value);

    [Fact]
    public void CreateJsonFileParser_When_Valid() {
        var option = new Option<Data>("--data") {
            CustomParser = Parsers.CreateJsonFileParser<Data>("option '--data'"),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--data", "./Data.json"]);

        Assert.Empty(result.Errors);
        Assert.Equal(new("Foo"), result.GetValue(option));
    }

    [Theory]
    [InlineData(new string[] { }, "Missing required argument for option '--data'.")]
    [InlineData(new string[] { "./Data.json", "./Empty.json" }, "Received too many arguments for option '--data'.")]
    [InlineData(new string[] { "./Missing.json" }, "Argument for option '--data' must be an existing file.")]
    [InlineData(new string[] { "./Empty.json" }, "Argument for option '--data' must be a valid JSON file.")]
    [InlineData(new string[] { "./Invalid.json" }, "Argument for option '--data' must be a valid JSON file: 'I' is an invalid start of a value. Path: $ | LineNumber: 0 | BytePositionInLine: 0.")]
    public void CreateJsonFileParser_When_Invalid(string[] arguments, string expectedError) {
        var option = new Option<Data>("--data") {
            CustomParser = Parsers.CreateJsonFileParser<Data>("option '--data'"),
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
    public void CreateSecondsParser_When_Valid(string argument, int expectedValue) {
        var option = new Option<int?>("--seconds") {
            CustomParser = Parsers.CreateSecondsParser("option '--seconds'"),
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
    public void CreateSecondsParser_When_Invalid(string[] arguments, string expectedError) {
        var option = new Option<int?>("--seconds") {
            CustomParser = Parsers.CreateSecondsParser("option '--seconds'"),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--seconds", .. arguments]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }
}
