using System.Net.Http;

namespace RequestFiend.Core;

public class TextContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection) {
        var content = new StringContent(collection.ApplyVariables(request.StringContent));

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
