using RequestFiend.Core;
using System.Text.RegularExpressions;

namespace RequestFiend.Console;

public class RequestFilter : IRequestFilter {
    private readonly Regex? include;
    private readonly Regex? exclude;

    public RequestFilter(Regex? include, Regex? exclude) {
        this.include = include;
        this.exclude = exclude;
    }

    public bool IsMatch(RequestTemplate request) {
        var shouldBeExecuted = true;

        if (include != null) {
            shouldBeExecuted &= include.IsMatch(request.Name);
        }

        if (exclude != null) {
            shouldBeExecuted &= !exclude.IsMatch(request.Name);
        }

        return shouldBeExecuted;
    }
};
