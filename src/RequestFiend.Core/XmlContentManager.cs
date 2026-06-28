using System.Net.Http;

namespace RequestFiend.Core;

public class XmlContentManager : IContentManager {
    public const string DefaultMediaType = "application/xml";

    public HttpContent? GetContent(RequestTemplateSnapshot request) {
        var content = new StringContent(request.Variables.Apply(request.StringContent), null, DefaultMediaType);

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
