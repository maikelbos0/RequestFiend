using MimeMapping;
using System.IO;
using System.Net.Http;

namespace RequestFiend.Core;

public class FormDataContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplateSnapshot request) {
        var content = new MultipartFormDataContent();

        foreach (var formField in request.FormFieldContent) {
            content.Add(new StringContent(request.Variables.Apply(formField.Value)) {
                Headers = {
                    ContentType = null
                }
            }, request.Variables.Apply(formField.Name));
        }

        foreach (var formFile in request.FormFileContent) {
            var filePath = request.Variables.Apply(formFile.Value);

            content.Add(new ByteArrayContent(File.ReadAllBytes(request.Variables.Apply(formFile.Value))) {
                Headers = {
                    ContentType = new(MimeUtility.GetMimeMapping(filePath))
                }
            }, request.Variables.Apply(formFile.Name));
        }

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
