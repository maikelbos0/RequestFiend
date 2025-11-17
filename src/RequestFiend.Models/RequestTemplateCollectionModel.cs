using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.Collections.Generic;
using System.Linq;

namespace RequestFiend.Models;

public class RequestTemplateCollectionModel : BoundModelBase {
    public OptionalString DefaultUrl { get; set; }
    public List<HeaderTemplateModel> DefaultHeaders { get; set; }

    public RequestTemplateCollectionModel(RequestTemplateCollection collection) {
        DefaultUrl = new(() => collection.DefaultUrl);
        DefaultHeaders = collection.DefaultHeaders.Select(x => new HeaderTemplateModel(x)).ToList();
    }

    public bool TryUpdateRequestTemplateCollection(RequestTemplateCollection collection) {
        var isValid = true;

        foreach (var header in DefaultHeaders) {
            isValid = isValid & header.Name.Validate() & header.Value.Validate();
        }

        if (!isValid) {
            return false;
        }

        collection.DefaultUrl = DefaultUrl;
        collection.DefaultHeaders = DefaultHeaders.Select(model => new HeaderTemplate() {
            Name = model.Name,
            Value = model.Value,
        }).ToList();
        
        return true;
    }
}
