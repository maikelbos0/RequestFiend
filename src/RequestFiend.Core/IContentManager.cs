using System.Net.Http;

namespace RequestFiend.Core;

public interface IContentManager {
    HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection);
}
