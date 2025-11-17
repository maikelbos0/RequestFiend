using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class RequestTemplateCollectionModel : BoundModelBase {
    public OptionalString DefaultUrl { get; set; }

    public RequestTemplateCollectionModel(RequestTemplateCollection collection) {
        DefaultUrl = new(() => collection.DefaultUrl);
    }

    public bool TryUpdateRequestTemplateCollection(RequestTemplateCollection collection) {
        collection.DefaultUrl = DefaultUrl;
        return true;
    }
}
