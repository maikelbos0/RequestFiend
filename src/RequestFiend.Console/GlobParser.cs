using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RequestFiend.Console;

public class GlobParser : IGlobParser {
    private static readonly HashSet<char> regexSpecialCharacters = ['.', '$', '^', '{', '[', '(', '|', ')', '*', '+', '?', '\\'];

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

                if (input.Length > i + 1 && input[i + 1] == '!') {
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
                    if (regexSpecialCharacters.Contains(input[i])) {
                        patternBuilder.Append('\\');
                    }

                    patternBuilder.Append(input[i]);
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
}
