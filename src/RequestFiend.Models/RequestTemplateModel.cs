using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.Linq;

namespace RequestFiend.Models;

public class RequestTemplateModel : BoundModelBase {
    private ContentType contentType;
    private bool usesStringContent;

    public RequiredString Name { get; set; }
    public RequiredString Method { get; set; }
    public RequiredString Url { get; set; }
    public NameValuePairModelCollection Headers { get; set; }
    public ContentType ContentType {
        get => contentType;
        set {
            if (SetProperty(ref contentType, value)) {
                UsesStringContent = contentType is ContentType.Text or ContentType.Json;
            }
        }
    }
    public bool UsesStringContent {
        get => usesStringContent;
        set => SetProperty(ref usesStringContent, value);
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
}
