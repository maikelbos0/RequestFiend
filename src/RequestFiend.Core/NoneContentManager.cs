using System.Net.Http;

namespace RequestFiend.Core;

public class NoneContentManager : IContentManager {
    public bool Validate(ContentTemplate contentTemplate, RequestTemplateCollection collection)
        => true;

    public bool Format(ContentTemplate content, RequestTemplateCollection collection)
        => true;

    public HttpContent? GetContent(ContentTemplate contentTemplate, RequestTemplateCollection collection)
        => null;
}
