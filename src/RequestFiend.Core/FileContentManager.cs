using System.IO;
using System.Net.Http;

namespace RequestFiend.Core;

public class FileContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection)
        => new ByteArrayContent(File.ReadAllBytes(collection.ApplyVariables(request.FilePathContent)));
}
