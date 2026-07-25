using System;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Text.Json;

namespace RequestFiend.Console;

public class ParserBuilder {
    private readonly IFileSystem fileSystem;

    public ParserBuilder(IFileSystem fileSystem) {
        this.fileSystem = fileSystem;
    }

    public Func<ArgumentResult, TValue?> BuildJsonFileParser<TValue>(string name) where TValue : class
        => result => {
            if (result.Tokens.Count == 0) {
                result.AddError($"Missing required argument for {name}.");
                return null;
            }

            if (result.Tokens.Count > 1) {
                result.AddError($"Received too many arguments for {name}.");
                return null;
            }

            if (!fileSystem.File.Exists(result.Tokens[0].Value)) {
                result.AddError($"Argument for {name} must be an existing file.");
                return null;
            }

            try {
                var value = JsonSerializer.Deserialize<TValue>(fileSystem.File.ReadAllText(result.Tokens[0].Value));

                if (value == null) {
                    result.AddError($"Argument for {name} must be a valid JSON file.");
                    return null;
                }

                return value;
            }
            catch (Exception exception) {
                result.AddError($"Argument for {name} must be a valid JSON file: {exception.Message}");
                return null;
            }
        };

    public Func<ArgumentResult, int?> BuildSecondsParser(string name)
        => result => {
            if (result.Tokens.Count == 0) {
                result.AddError($"Missing required argument for {name}.");
                return null;
            }

            if (result.Tokens.Count > 1) {
                result.AddError($"Received too many arguments for {name}.");
                return null;
            }

            if (!int.TryParse(result.Tokens[0].Value, out var seconds) || seconds < 1) {
                result.AddError($"Argument for {name} must be a positive number of seconds.");
                return null;
            }

            return seconds;
        };
}
