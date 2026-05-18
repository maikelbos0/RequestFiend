using System.Net.Http;

namespace RequestFiend.Core;

public class XmlContentManager : IContentManager {
    public const string DefaultMediaType = "application/xml";

    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection)
        => new StringContent(collection.ApplyVariables(request.StringContent), null, DefaultMediaType);
}
