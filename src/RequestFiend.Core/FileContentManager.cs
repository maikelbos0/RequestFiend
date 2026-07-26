using MimeMapping;
using System.IO.Abstractions;
using System.Net.Http;

namespace RequestFiend.Core;

public class FileContentManager : IContentManager {
    private readonly IFileSystem fileSystem;

    public FileContentManager(IFileSystem fileSystem) {
        this.fileSystem = fileSystem;
    }

    public HttpContent? GetContent(RequestTemplateSnapshot request) {
        var filePath = request.Variables.Apply(request.FileContent);
        var content = new ByteArrayContent(fileSystem.File.ReadAllBytes(filePath));

        if (!request.HasManualContentTypeHeader) {
            content.Headers.ContentType = new(MimeUtility.GetMimeMapping(filePath));
        }

        return content;
    }
}
