using System;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RequestFiend.Console;

public class ParserBuilder {
    private readonly IFileSystem fileSystem;
    private readonly IGlobParser globParser;

    public ParserBuilder(IFileSystem fileSystem, IGlobParser globParser) {
        this.fileSystem = fileSystem;
        this.globParser = globParser;
    }

    public Func<ArgumentResult, TValue?> BuildJsonFileParser<TValue>() where TValue : class
        => result => {
            if (result.Tokens.Count != 1) {
                throw new ArgumentException("Parser must be called with a single argument.");
            }

            var name = GetName(result);

            if (!fileSystem.File.Exists(result.Tokens[0].Value)) {
                result.AddError($"{name} must be an existing file.");
                return null;
            }

            try {
                var value = JsonSerializer.Deserialize<TValue>(fileSystem.File.ReadAllText(result.Tokens[0].Value));

                if (value == null) {
                    result.AddError($"{name} must be a valid JSON file.");
                    return null;
                }

                return value;
            }
            catch (Exception exception) {
                result.AddError($"{name} must be a valid JSON file: {exception.Message}");
                return null;
            }
        };

    public Func<ArgumentResult, int?> BuildSecondsParser()
        => result => {
            if (result.Tokens.Count != 1) {
                throw new ArgumentException("Parser must be called with a single argument.");
            }

            var name = GetName(result);

            if (!int.TryParse(result.Tokens[0].Value, out var seconds) || seconds < 1) {
                result.AddError($"{name} must be a positive number of seconds.");
                return null;
            }

            return seconds;
        };

    public Func<ArgumentResult, Regex?> BuildGlobParser()
        => result => {
            if (result.Tokens.Count != 1) {
                throw new ArgumentException("Parser must be called with a single argument.");
            }

            var name = GetName(result);

            try {
                if (!globParser.TryParse(result.Tokens[0].Value, out var pattern)) {
                    result.AddError($"{name} must be a valid glob pattern.");
                    return null;
                }

                return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            catch {
                result.AddError($"{name} must be a valid glob pattern.");
                return null;
            }
        };

    private static string GetName(ArgumentResult result) {
        if (result.Parent is OptionResult optionResult) {
            return $"Argument for option '{optionResult.IdentifierToken?.Value ?? optionResult.Option.Name}'";
        }

        return $"Argument '{result.Argument.Name}'";
    }
}
