using System.Net.Http;

namespace RequestFiend.Core;

public class JsonContentManager : IContentManager {
    public const string DefaultMediaType = "text/plain";

    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection) {
        if (string.IsNullOrEmpty(request.StringContent)) {
            return null;
        }

        return new StringContent(collection.ApplyVariables(request.StringContent), null, DefaultMediaType);
    }
}
