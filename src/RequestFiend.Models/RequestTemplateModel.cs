using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class RequestTemplateModel : BoundModelBase {

    public RequiredString Name { get; set; }
    public RequiredString Method { get; set; }
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
}
