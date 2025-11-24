using System.Net.Http;

namespace RequestFiend.Core;

public class TextContentManager : IContentManager {
    public const string DefaultMediaType = "application/json";

    public HttpContent? GetContent(ContentTemplate content, RequestTemplateCollection collection)
        => new StringContent(collection.ApplyVariables(content.StringContent));
}
