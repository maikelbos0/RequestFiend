using System.Collections.Generic;
using System.Net.Http;

namespace RequestFiend.Core;

// TODO should this be integrated into RequestTemplate?
public class ContentTemplate {
    private static Dictionary<ContentType, IContentManager> managers = new() {
        [ContentType.None] = new NoneContentManager(),
        [ContentType.Text] = new TextContentManager(),
        [ContentType.Json] = new JsonContentManager(),
    };

    public ContentType Type { get; set; } = ContentType.None;
    public IContentManager Manager => managers[Type];
    
    // TODO could this be nullable anyway?
    public string StringContent { get; set; } = string.Empty;

    // TODO this will only be used in the UI, does it make sense here?
    public bool Validate(RequestTemplateCollection collection)
        => Manager.Validate(this, collection);

    // TODO this will only be used in the UI, does it make sense here?
    public bool Format(RequestTemplateCollection collection)
        => Manager.Format(this, collection);

    public HttpContent? GetContent(RequestTemplateCollection collection)
        => Manager.GetContent(this, collection);
}
