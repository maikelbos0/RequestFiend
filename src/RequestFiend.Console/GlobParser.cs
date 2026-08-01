using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RequestFiend.Console;

public class GlobParser {
    private static readonly HashSet<char> regexSpecialCharacters = [ '.', '$', '^', '{', '[', '(', '|', ')', '*', '+', '?', '\\'];

    public bool TryParse(string input, [NotNullWhen(true)] out string? pattern) {
        var patternBuilder = new StringBuilder();
        var isInSet = false;
        int startOfSet = 0;
        
        for (var i = 0; i < input.Length; i++) {
            if (input[i] == '?' && !isInSet) {
                patternBuilder.Append('.');
            }
            else if (input[i] == '*' && !isInSet) {
                patternBuilder.Append(".*");
            }
            else if (input[i] == '[' && !isInSet) {
                isInSet = true;
                patternBuilder.Append(input[i]);

                if (input.Length > i + 1 && input[i+1]== '!') {
                    i++;
                    patternBuilder.Append('^');
                }

                startOfSet = i;
            }
            else if (input[i] == ']' && isInSet) {
                if (startOfSet == i - 1) {
                    pattern = null;
                    return false;
                }

                isInSet = false;
                patternBuilder.Append(input[i]);
            }
            else {
                if (input[i] == '`') {
                    i++;
                }

                if (input.Length > i) {
                    AppendCharacter(patternBuilder, input[i]);
                }
            }
        }

        if (isInSet) {
            pattern = null;
            return false;
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
