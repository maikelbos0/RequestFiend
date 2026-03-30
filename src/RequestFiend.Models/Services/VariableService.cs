using RequestFiend.Core;
using System;
using System.Collections.Generic;

namespace RequestFiend.Models.Services;

public static class VariableService {
    public static IEnumerable<TValue> ProcessText<TValue>(string text, Func<string,TValue> textProcessor, Func<string, TValue> variableReferenceProcessor) {
        var previousPosition = 0;

        for (var position = 0; position < text.Length; position++) {
            if (TryGetVariableReferenceLength(text, position, out var length)) {
                if (position > previousPosition) {
                    yield return textProcessor(text.Substring(previousPosition, position - previousPosition));
                }

                yield return variableReferenceProcessor(text.Substring(position, length));
                previousPosition = position + length;
            }
        }

        if (previousPosition <= text.Length - 1) {
            yield return textProcessor(text.Substring(previousPosition));
        }
    }

    private static bool TryGetVariableReferenceLength(string text, int position, out int length) {
        if (text.Length > position + 4 && text[position] == '{' && text[position + 1] == '{') {
            length = 4;

            while (text.Length > position + length && RequestTemplateCollection.IsValidVariableCharacter(text[position + length - 2])) {
                length++;
            }

            return text[position + length - 1] == '}' && text[position + length - 2] == '}';
        }

        length = 0;
        return false;
    }
}
