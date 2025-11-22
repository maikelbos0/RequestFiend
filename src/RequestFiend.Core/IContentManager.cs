using System.Net.Http;

namespace RequestFiend.Core;

public interface IContentManager {
    bool Validate(ContentTemplate contentTemplate, RequestTemplateCollection collection);
    bool Format(ContentTemplate contentTemplate, RequestTemplateCollection collection);
    HttpContent? GetContent(ContentTemplate contentTemplate, RequestTemplateCollection collection);
}
