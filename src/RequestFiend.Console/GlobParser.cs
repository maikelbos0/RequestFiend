using System.Collections.Generic;
﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RequestFiend.Console;

public class GlobParser {
    private const char singleCharacterMatch = '?';
    private const char zeroOrMoreCharactersMatch = '*';
    private static readonly HashSet<char> regexSpecialCharacters = [ '.', '$', '^', '{', '(', '|', ')', '+', '\\'];

    // . $ ^ { [ ( | ) * + ? \
    public bool TryParse(string input, [NotNullWhen(true)] out string? pattern) {
        var patternBuilder = new StringBuilder();
        
        for (var i = 0; i < input.Length; i++) {
            if (input[i] == singleCharacterMatch) {
                patternBuilder.Append('.');
            }
            else if (input[i] == zeroOrMoreCharactersMatch) {
                patternBuilder.Append(".*");
            }
            else {
                AppendCharacter(patternBuilder, input[i]);
            }
        }

        pattern = patternBuilder.ToString();
        return true;
    }

    public void AppendCharacter(StringBuilder patternBuilder, char input) {
        if (regexSpecialCharacters.Contains(input)) {
            patternBuilder.Append('\\');
        }

        patternBuilder.Append(input);
    }
}
