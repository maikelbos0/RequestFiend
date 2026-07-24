using System;
using System.CommandLine.Parsing;

namespace RequestFiend.Console;

public static class Parsers {
    public static Func<ArgumentResult, TimeSpan?> CreateSecondsParser(string name)
        => (ArgumentResult result) => {
            if (result.Tokens.Count == 0) {
                result.AddError($"Missing required argument for {name}");
                return null;
            }

            if (result.Tokens.Count > 1) {
                result.AddError($"Received too many arguments for {name}");
                return null;
            }
            
            if (!int.TryParse(result.Tokens[0].Value, out var seconds) || seconds < 1) {
                result.AddError($"Argument for {name} must be a positive number of seconds");
                return null;
            }

            return TimeSpan.FromSeconds(seconds);
        };
}
