using System.Net.Http;

namespace RequestFiend.Core;

public interface IContentManager {
    HttpContent? GetContent(ContentTemplate contentTemplate, RequestTemplateCollection collection);
}
