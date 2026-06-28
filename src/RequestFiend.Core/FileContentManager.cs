using MimeMapping;
using System.IO;
using System.Net.Http;

namespace RequestFiend.Core;

public class FileContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplateSnapshot request) {
        var filePath = request.Variables.Apply(request.FileContent);
        var content = new ByteArrayContent(File.ReadAllBytes(filePath));

        if (!request.HasManualContentTypeHeader) {
            content.Headers.ContentType = new(MimeUtility.GetMimeMapping(filePath));
        }

        return content;
    }
}
