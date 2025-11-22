using System.Collections.Generic;
using System.Net.Http;

namespace RequestFiend.Core;

public class ContentTemplate {
    private static Dictionary<ContentType, IContentManager> managers = new() {
        [ContentType.None] = new NoneContentManager(),
        [ContentType.Text] = new TextContentManager(),
        [ContentType.Json] = new JsonContentManager(),
    };

    public ContentType Type { get; set; } = ContentType.None;
    public IContentManager Manager => managers[Type];
    public string StringContent { get; set; } = string.Empty;

    public bool Validate(RequestTemplateCollection collection)
        => Manager.Validate(this, collection);

    public bool Format(RequestTemplateCollection collection)
        => Manager.Format(this, collection);

    public HttpContent? GetContent(RequestTemplateCollection collection)
        => Manager.GetContent(this, collection);
}
