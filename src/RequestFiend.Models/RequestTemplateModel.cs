using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class RequestTemplateModel : ModelBase {

    public RequiredString Name { get; set; } = new();
    public RequiredString Method { get; set; } = new();
    public RequiredString Url { get; set; }

    public RequestTemplateModel(RequestTemplate requestTemplate) {
        Name = new(() => requestTemplate.Name);
        Method = new(() => requestTemplate.Method);
        Url = new(() => requestTemplate.Url);
    }

    public bool TryUpdateRequestTemplate(RequestTemplate request) {
        if (!Name.Validate() | !Method.Validate() | !Url.Validate()) {
            return false;
        }

        request.Name = Name;
        request.Method = Method;
        request.Url = Url;
        return true;
    }

    public void Reset() {
        Name.Reset();
        Method.Reset();
        Url.Reset();
    }
}
