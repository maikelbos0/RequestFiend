using MimeMapping;
using System.IO;
using System.Net.Http;

namespace RequestFiend.Core;

public class FormContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplate request, RequestTemplateCollection collection) {
        var content = new MultipartFormDataContent();

        foreach (var formFieldItem in request.FormFieldContent) {
            content.Add(new StringContent(collection.ApplyVariables(formFieldItem.Value)) {
                Headers = { 
                    ContentType = null 
                } 
            }, collection.ApplyVariables(formFieldItem.Name));
        }

        foreach (var formFileItem in request.FormFileContent) {
            var filePath = collection.ApplyVariables(formFileItem.Value);

            content.Add(new ByteArrayContent(File.ReadAllBytes(collection.ApplyVariables(formFileItem.Value))) {
                Headers = {
                    ContentType = new(MimeUtility.GetMimeMapping(filePath))
                }
            }, collection.ApplyVariables(formFileItem.Name));
        }

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
