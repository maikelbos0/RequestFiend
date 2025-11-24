using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace RequestFiend.Models;

public class RequestTemplateModel : BoundModelBase {
    private ContentType contentType;
    private bool usesStringContent;
    private bool usesJsonContent;

    public RequiredString Name { get; set; }
    public RequiredString Method { get; set; }
    public RequiredString Url { get; set; }
    public NameValuePairModelCollection Headers { get; set; }
    public ContentType ContentType {
        get => contentType;
        set {
            if (SetProperty(ref contentType, value)) {
                UsesStringContent = contentType is ContentType.Text or ContentType.Json;
                UsesJsonContent = contentType is ContentType.Json;
            }
        }
    }
    public bool UsesStringContent {
        get => usesStringContent;
        set => SetProperty(ref usesStringContent, value);
    }
    public bool UsesJsonContent {
        get => usesJsonContent;
        set => SetProperty(ref usesJsonContent, value);
    }
    public OptionalString StringContent { get; set; }

    public RequestTemplateModel(RequestTemplate request) {
        Name = new(() => request.Name);
        Method = new(() => request.Method);
        Url = new(() => request.Url);
        Headers = [.. request.Headers.Select(pair => new NameValuePairModel(pair))];
        ContentType = request.Content.Type;
        StringContent = new(() => request.Content.StringContent);
    }

    public bool TryUpdateRequestTemplate(RequestTemplate request) {
        if (!Name.IsValid || !Method.IsValid || !Url.IsValid || Headers.Any(header => !header.IsValid)) {
            return false;
        }

        request.Name = Name;
        request.Method = Method;
        request.Url = Url;
        request.Headers = [.. Headers.Select(header => new NameValuePair() { Name = header.Name, Value = header.Value })];
        request.Content.Type = ContentType;
        request.Content.StringContent = StringContent!;
        return true;
    }

    public bool ValidateJson([NotNullWhen(false)] out Exception? exception) {
        try {
            _ = JsonDocument.Parse(StringContent.Value ?? "");
            exception = null;
            return true;
        }
        catch (Exception ex) {
            exception = ex;
            return false;
        }
    }
}
