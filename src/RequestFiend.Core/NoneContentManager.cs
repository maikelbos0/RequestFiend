using System.Net.Http;

namespace RequestFiend.Core;

public class NoneContentManager : IContentManager {
    public HttpContent? GetContent(ContentTemplate contentTemplate, RequestTemplateCollection collection)
        => null;
}
