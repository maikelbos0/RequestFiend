using System.Net.Http;

namespace RequestFiend.Core;

public class JsonContentManager : IContentManager {
    public const string DefaultMediaType = "text/plain";

    public HttpContent? GetContent(RequestTemplate request, VariableSnapshot variableSnapshot) {
        var content = new StringContent(variableSnapshot.Apply(request.StringContent), null, DefaultMediaType);

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
