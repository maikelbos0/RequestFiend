using System;
using System.CommandLine;
using System.Linq;
using Xunit;

namespace RequestFiend.Console.Tests;

public class ParsersTests {
    [Theory]
    [InlineData("1", 1)]
    [InlineData("123", 123)]
    public void CreateSecondsParser_When_Valid(string argument, int expectedValueInSeconds) {
        var option = new Option<TimeSpan?>("--seconds") {
            CustomParser = Parsers.CreateSecondsParser("option '--seconds'"),
            Arity = ArgumentArity.ZeroOrMore
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--seconds", argument]);

        Assert.Empty(result.Errors);
        Assert.Equal(TimeSpan.FromSeconds(expectedValueInSeconds), result.GetValue(option));
    }

    [Theory]
    [InlineData("option '--seconds'", new string[] { }, new string[] { "Missing required argument for option '--seconds'" })]
    [InlineData("option '--seconds'", new string[] { "1", "1" }, new string[] { "Received too many arguments for option '--seconds'" })]
    [InlineData("option '--seconds'", new string[] { "" }, new string[] { "Argument for option '--seconds' must be a positive number of seconds" })]
    [InlineData("option '--seconds'", new string[] { "1a" }, new string[] { "Argument for option '--seconds' must be a positive number of seconds" })]
    [InlineData("option '--seconds'", new string[] { "0" }, new string[] { "Argument for option '--seconds' must be a positive number of seconds" })]
    public void CreateSecondsParser_When_Invalid(string name, string[] arguments, string[] expectedErrors) {
        var option = new Option<TimeSpan?>("--seconds") {
            CustomParser = Parsers.CreateSecondsParser(name),
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--seconds", .. arguments]);

        Assert.Equivalent(expectedErrors, result.Errors.Select(error => error.Message));
    }
}
