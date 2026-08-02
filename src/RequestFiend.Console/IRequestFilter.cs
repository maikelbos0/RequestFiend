using RequestFiend.Core;

namespace RequestFiend.Console;

public interface IRequestFilter {
    bool IsMatch(RequestTemplate request);
}
