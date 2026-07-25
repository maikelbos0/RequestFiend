using System;
using System.CommandLine;
using Xunit;

namespace RequestFiend.Console.Tests;

public class ParsersTests {
    [Theory]
    [InlineData("1", 1)]
    [InlineData("123", 123)]
    public void CreateSecondsParser_When_Valid(string argument, int expectedValueInSeconds) {
        var option = new Option<TimeSpan?>("--seconds") {
            CustomParser = Parsers.CreateSecondsParser("option '--seconds'"),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--seconds", argument]);

        Assert.Empty(result.Errors);
        Assert.Equal(TimeSpan.FromSeconds(expectedValueInSeconds), result.GetValue(option));
    }

    [Theory]
    [InlineData(new string[] { }, "Missing required argument for option '--seconds'.")]
    [InlineData(new string[] { "1", "1" }, "Received too many arguments for option '--seconds'.")]
    [InlineData(new string[] { "" }, "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData(new string[] { "1a" }, "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData(new string[] { "0" }, "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData(new string[] { "-1" }, "Argument for option '--seconds' must be a positive number of seconds.")]
    public void CreateSecondsParser_When_Invalid(string[] arguments, string expectedError) {
        var option = new Option<TimeSpan?>("--seconds") {
            CustomParser = Parsers.CreateSecondsParser("option '--seconds'"),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--seconds", .. arguments]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }
}
