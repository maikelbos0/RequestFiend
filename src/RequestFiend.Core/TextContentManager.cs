using System.Net.Http;

namespace RequestFiend.Core;

public class TextContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplate request, VariableSnapshot variableSnapshot) {
        var content = new StringContent(variableSnapshot.Apply(request.StringContent));

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }

    public HttpContent? GetContent(RequestTemplateSnapshot request) {
        var content = new StringContent(request.Variables.Apply(request.StringContent));

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
