using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class HeaderTemplateModel {
    public RequiredString Name { get; set; }
    public RequiredString Value { get; set; }

    public HeaderTemplateModel(HeaderTemplate header) {
        Name = new(() => header.Name);
        Value = new(() => header.Value);
    }
}
