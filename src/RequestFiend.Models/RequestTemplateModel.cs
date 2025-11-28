using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace RequestFiend.Models;

public class RequestTemplateModel : BoundModelBase {
    private static JsonSerializerOptions jsonSerializerOptions { get; } = new() { WriteIndented = true };

    private ContentType contentType;
    private bool usesStringContent;
    private bool usesJsonContent;

    public ValidatableString Name { get; set; }
    public ValidatableString Method { get; set; }
    public ValidatableString Url { get; set; }
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
    public ValidatableString StringContent { get; set; }

    public RequestTemplateModel(RequestTemplate request) {
        Name = new(true, () => request.Name);
        Method = new(true, () => request.Method);
        Url = new(true, () => request.Url);
        Headers = [.. request.Headers.Select(pair => new NameValuePairModel(pair))];
        ContentType = request.ContentType;
        StringContent = new(false, () => request.StringContent);
    }

    public bool TryUpdateRequestTemplate(RequestTemplate request) {
        if (Name.HasError || Method.HasError || Url.HasError|| Headers.Any(header => !header.IsValid)) {
            return false;
        }

        request.Name = Name.Value!;
        request.Method = Method.Value!;
        request.Url = Url.Value!;
        request.Headers = [.. Headers.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        request.ContentType = ContentType;
        request.StringContent = StringContent.Value;
        return true;
    }

    public bool ValidateJson([NotNullWhen(false)] out Exception? exception) {
        try {
            if (!string.IsNullOrEmpty(StringContent.Value)) {
                _ = JsonDocument.Parse(StringContent.Value ?? "");
            }
            exception = null;
            return true;
        }
        catch (Exception ex) {
            exception = ex;
            return false;
        }
    }

    public bool FormatJson([NotNullWhen(false)] out Exception? exception) {
        try {
            if (!string.IsNullOrEmpty(StringContent.Value)) {
                var document = JsonDocument.Parse(StringContent.Value ?? "");
                StringContent.Value = JsonSerializer.Serialize(document, jsonSerializerOptions);
            }
            exception = null;
            return true;
        }
        catch (Exception ex) {
            exception = ex;
            return false;
        }
    }
}
