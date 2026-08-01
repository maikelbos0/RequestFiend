using NSubstitute;
using System.CommandLine;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        var subject = new ParserBuilder(fileSystem, Substitute.For<IGlobParser>());

        var option = new Option<Data>("--data") {
            CustomParser = subject.BuildJsonFileParser<Data>()
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--data", "./Data.json"]);

        Assert.Empty(result.Errors);
        Assert.Equal(data, result.GetValue(option));
    }

    [Theory]
    [InlineData(false, null, "Argument for option '--data' must be an existing file.")]
    [InlineData(true, "null", "Argument for option '--data' must be a valid JSON file.")]
    [InlineData(true, "Invalid", "Argument for option '--data' must be a valid JSON file: 'I' is an invalid start of a value. Path: $ | LineNumber: 0 | BytePositionInLine: 0.")]
    public void BuildJsonFileParser_When_Invalid_Option(bool fileExists, string? fileContents, string expectedError) {
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists("./Data.json").Returns(fileExists);
        fileSystem.File.ReadAllText("./Data.json").Returns(fileContents);

        var subject = new ParserBuilder(fileSystem, Substitute.For<IGlobParser>());

        var command = new RootCommand() {
            new Option<Data>("--data") {
                CustomParser = subject.BuildJsonFileParser<Data>()
            }
        };

        var result = command.Parse(["--data", "./Data.json"]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }

    [Theory]
    [InlineData(false, null, "Argument 'data' must be an existing file.")]
    [InlineData(true, "null", "Argument 'data' must be a valid JSON file.")]
    [InlineData(true, "Invalid", "Argument 'data' must be a valid JSON file: 'I' is an invalid start of a value. Path: $ | LineNumber: 0 | BytePositionInLine: 0.")]
    public void BuildJsonFileParser_When_Invalid_Argument(bool fileExists, string? fileContents, string expectedError) {
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.File.Exists("./Data.json").Returns(fileExists);
        fileSystem.File.ReadAllText("./Data.json").Returns(fileContents);

        var subject = new ParserBuilder(fileSystem, Substitute.For<IGlobParser>());

        var command = new RootCommand() {
            new Argument<Data>("data") {
                CustomParser = subject.BuildJsonFileParser<Data>()
            }
        };

        var result = command.Parse(["./Data.json"]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("123", 123)]
    public void BuildSecondsParser_When_Valid(string argument, int expectedValue) {
        var subject = new ParserBuilder(Substitute.For<IFileSystem>(), Substitute.For<IGlobParser>());

        var option = new Option<int?>("--seconds") {
            CustomParser = subject.BuildSecondsParser()
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--seconds", argument]);

        Assert.Empty(result.Errors);
        Assert.Equal(expectedValue, result.GetValue(option));
    }

    [Theory]
    [InlineData("", "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData("1a", "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData("0", "Argument for option '--seconds' must be a positive number of seconds.")]
    [InlineData("-1", "Argument for option '--seconds' must be a positive number of seconds.")]
    public void BuildSecondsParser_When_Invalid_Option(string argument, string expectedError) {
        var subject = new ParserBuilder(Substitute.For<IFileSystem>(), Substitute.For<IGlobParser>());

        var command = new RootCommand() {
            new Option<int?>("--seconds") {
                CustomParser = subject.BuildSecondsParser()
            }
        };

        var result = command.Parse(["--seconds", argument]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }

    [Theory]
    [InlineData("", "Argument 'seconds' must be a positive number of seconds.")]
    [InlineData("1a", "Argument 'seconds' must be a positive number of seconds.")]
    [InlineData("0", "Argument 'seconds' must be a positive number of seconds.")]
    [InlineData("-1", "Argument 'seconds' must be a positive number of seconds.")]
    public void BuildSecondsParser_When_Invalid_Argument(string argument, string expectedError) {
        var subject = new ParserBuilder(Substitute.For<IFileSystem>(), Substitute.For<IGlobParser>());

        var command = new RootCommand() {
            new Argument<int?>("seconds") {
                CustomParser = subject.BuildSecondsParser()
            }
        };

        var result = command.Parse([argument]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }

    [Fact]
    public void BuildGlobParser_When_Valid() {
        var globParser = Substitute.For<IGlobParser>();
        globParser.TryParse("argu*", out _).Returns(callInfo => {
            callInfo[1] = "argu.*";
            return true;
        });

        var subject = new ParserBuilder(Substitute.For<IFileSystem>(), globParser);

        var option = new Option<Regex?>("--filter") {
            CustomParser = subject.BuildGlobParser()
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--filter", "argu*"]);

        Assert.Empty(result.Errors);

        var regex = result.GetValue(option);
        Assert.NotNull(regex);
        Assert.Equal("argu.*", regex.ToString());
        Assert.Equal(RegexOptions.IgnoreCase | RegexOptions.Compiled, regex.Options);
    }

    [Theory]
    [InlineData(false, null, "Argument for option '--filter' must be a valid glob pattern.")]
    [InlineData(true, "[]", "Argument for option '--filter' must be a valid glob pattern.")]
    public void BuildGlobParser_When_Invalid_Option(bool isValid, string? pattern, string expectedError) {
        var globParser = Substitute.For<IGlobParser>();
        globParser.TryParse("argu*", out _).Returns(callInfo => {
            callInfo[1] = pattern;
            return isValid;
        });

        var subject = new ParserBuilder(Substitute.For<IFileSystem>(), globParser);

        var option = new Option<Regex?>("--filter") {
            CustomParser = subject.BuildGlobParser()
        };

        var command = new RootCommand() { option };

        var result = command.Parse(["--filter", "argu*"]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }

    [Theory]
    [InlineData(false, null, "Argument 'filter' must be a valid glob pattern.")]
    [InlineData(true, "[]", "Argument 'filter' must be a valid glob pattern.")]
    public void BuildGlobParser_When_Invalid_Argument(bool isValid, string? pattern, string expectedError) {
        var globParser = Substitute.For<IGlobParser>();
        globParser.TryParse("argu*", out _).Returns(callInfo => {
            callInfo[1] = pattern;
            return isValid;
        });

        var subject = new ParserBuilder(Substitute.For<IFileSystem>(), globParser);

        var command = new RootCommand() {
            new Argument<Regex?>("filter") {
                CustomParser = subject.BuildGlobParser()
            }
        };

        var result = command.Parse(["argu*"]);

        Assert.Equivalent(expectedError, Assert.Single(result.Errors).Message);
    }
}
