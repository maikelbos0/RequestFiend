using RequestFiend.Core;
using RequestFiend.UI.Models.Properties;

namespace RequestFiend.UI.Models;

public class RequestTemplateCollectionModel {
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
