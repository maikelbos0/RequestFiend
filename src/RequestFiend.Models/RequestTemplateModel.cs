using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.Linq;

namespace RequestFiend.Models;

public class RequestTemplateModel : BoundModelBase {
    public RequiredString Name { get; set; }
    public RequiredString Method { get; set; }
    public RequiredString Url { get; set; }
    public ContentType ContentType { get; set; }
    public NameValuePairModelCollection Headers { get; set; }

    public RequestTemplateModel(RequestTemplate request) {
        Name = new(() => request.Name);
        Method = new(() => request.Method);
        Url = new(() => request.Url);
        ContentType = request.Content.Type;
        Headers = [.. request.Headers.Select(pair => new NameValuePairModel(pair))];
    }

    public bool TryUpdateRequestTemplate(RequestTemplate request) {
        if (!Name.IsValid || !Method.IsValid || !Url.IsValid || Headers.Any(header => !header.IsValid)) {
            return false;
        }

        request.Name = Name;
        request.Method = Method;
        request.Url = Url;
        request.Content.Type = ContentType;
        request.Headers = [.. Headers.Select(header => new NameValuePair() { Name = header.Name, Value = header.Value })];
        return true;
    }
}
