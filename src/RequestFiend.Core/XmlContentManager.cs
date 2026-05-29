using System.Net.Http;

namespace RequestFiend.Core;

public class XmlContentManager : IContentManager {
    public const string DefaultMediaType = "application/xml";

    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection) {
        var content = new StringContent(collection.ApplyVariables(request.StringContent), null, DefaultMediaType);

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
