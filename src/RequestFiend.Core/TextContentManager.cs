using System.Net.Http;

namespace RequestFiend.Core;

public class TextContentManager : IContentManager {
    public const string DefaultMediaType = "application/json";

    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection) {
        if (string.IsNullOrEmpty(request.StringContent)) {
            return null;
        }

        return new StringContent(collection.ApplyVariables(request.StringContent));
    }
}
