using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.Models;

public class RequestTemplateCollectionModel : ModelBase {
    public OptionalString DefaultUrl { get; set; } = new();

    public RequestTemplateCollectionModel(RequestTemplateCollection collection) {
        DefaultUrl = new(() => collection.DefaultUrl);
    }

    public bool TryUpdateRequestTemplateCollection(RequestTemplateCollection collection) {
        collection.DefaultUrl = DefaultUrl;
        return true;
    }

    public void Reset() {
        DefaultUrl.Reset();
    }
}
