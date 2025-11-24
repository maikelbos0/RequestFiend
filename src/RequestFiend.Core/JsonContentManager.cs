using System.Net.Http;

namespace RequestFiend.Core;

public class JsonContentManager : IContentManager {
    public const string DefaultMediaType = "text/plain";

    public HttpContent? GetContent(ContentTemplate content, RequestTemplateCollection collection)
        => new StringContent(collection.ApplyVariables(content.StringContent), null, DefaultMediaType);
}
