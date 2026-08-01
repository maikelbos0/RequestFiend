using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Console;

public interface IGlobParser {
    bool TryParse(string input, [NotNullWhen(true)] out string? pattern);
}