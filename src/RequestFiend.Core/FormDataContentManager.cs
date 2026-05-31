using MimeMapping;
using System.IO;
using System.Net.Http;

namespace RequestFiend.Core;

public class FormDataContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection) {
        var content = new MultipartFormDataContent();

        foreach (var formField in request.FormFieldContent) {
            content.Add(new StringContent(collection.ApplyVariables(formField.Value)) {
                Headers = { 
                    ContentType = null 
                } 
            }, collection.ApplyVariables(formField.Name));
        }

        foreach (var formFile in request.FormFileContent) {
            var filePath = collection.ApplyVariables(formFile.Value);

            content.Add(new ByteArrayContent(File.ReadAllBytes(collection.ApplyVariables(formFile.Value))) {
                Headers = {
                    ContentType = new(MimeUtility.GetMimeMapping(filePath))
                }
            }, collection.ApplyVariables(formFile.Name));
        }

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
