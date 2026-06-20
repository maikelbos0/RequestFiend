using MimeMapping;
using System.IO;
using System.Net.Http;

namespace RequestFiend.Core;

public class FormDataContentManager : IContentManager {
    public HttpContent? GetContent(RequestTemplate request, VariableSnapshot variableSnapshot) {
        var content = new MultipartFormDataContent();

        foreach (var formField in request.FormFieldContent) {
            content.Add(new StringContent(variableSnapshot.Apply(formField.Value)) {
                Headers = { 
                    ContentType = null 
                } 
            }, variableSnapshot.Apply(formField.Name));
        }

        foreach (var formFile in request.FormFileContent) {
            var filePath = variableSnapshot.Apply(formFile.Value);

            content.Add(new ByteArrayContent(File.ReadAllBytes(variableSnapshot.Apply(formFile.Value))) {
                Headers = {
                    ContentType = new(MimeUtility.GetMimeMapping(filePath))
                }
            }, variableSnapshot.Apply(formFile.Name));
        }

        if (request.HasManualContentTypeHeader) {
            content.Headers.ContentType = null;
        }

        return content;
    }
}
