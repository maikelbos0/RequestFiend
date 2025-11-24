using System.Net.Http;

namespace RequestFiend.Core;

public class NoneContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection)
        => null;
}
