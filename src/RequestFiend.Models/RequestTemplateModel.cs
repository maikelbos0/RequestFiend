using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.Collections.ObjectModel;
using System.Linq;

namespace RequestFiend.Models;

public class RequestTemplateModel : BoundModelBase {

    public RequiredString Name { get; set; }
    public RequiredString Method { get; set; }
    public RequiredString Url { get; set; }
    public ObservableCollection<NameValuePairModel> Headers { get; set; }

    public RequestTemplateModel(RequestTemplate request) {
        Name = new(() => request.Name);
        Method = new(() => request.Method);
        Url = new(() => request.Url);
        Headers = new(request.Headers.Select(pair => new NameValuePairModel(pair)));
    }

    public bool TryUpdateRequestTemplate(RequestTemplate request) {
        var isValid = Name.Validate() & Method.Validate() & Url.Validate();

        foreach (var header in Headers) {
            isValid = isValid & header.Name.Validate() & header.Value.Validate();
        }

        if (!isValid) {
            return false;
        }

        request.Name = Name;
        request.Method = Method;
        request.Url = Url;
        request.Headers = [.. Headers.Select(header => new NameValuePair() { Name = header.Name, Value = header.Value })];
        return true;
    }
}
