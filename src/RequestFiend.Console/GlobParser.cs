using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RequestFiend.Console;

public class GlobParser {
    private const char singleCharacterMatch = '?';
    private const char zeroOrMoreCharactersMatch = '*';

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
                patternBuilder.Append(input[i]);
            }
        }

        pattern = patternBuilder.ToString();
        return true;
    }
}
