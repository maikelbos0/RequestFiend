using System.Net.Http;

namespace RequestFiend.Core;

public class TextContentManager : IContentManager {
    public const string DefaultMediaType = "application/json";

    public bool Validate(ContentTemplate content, RequestTemplateCollection collection)
        => true;

    public bool Format(ContentTemplate content, RequestTemplateCollection collection)
        => true;

    public HttpContent? GetContent(ContentTemplate content, RequestTemplateCollection collection)
        => new StringContent(collection.ApplyVariables(content.StringContent));
}
