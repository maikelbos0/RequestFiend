using System.Net.Http;

namespace RequestFiend.Core;

public class NoneContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplate request, VariableSnapshot variableSnapshot)
        => null;

    public HttpContent? GetContent(RequestTemplateSnapshot request)
        => null;
}
