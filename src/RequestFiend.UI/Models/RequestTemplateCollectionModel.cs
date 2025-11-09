using RequestFiend.Core;
using RequestFiend.UI.Models.Properties;

namespace RequestFiend.UI.Models;

public class RequestTemplateCollectionModel {
    public RequiredString Name { get; set; } = new();
    public OptionalString DefaultUrl { get; set; } = new();

    public RequestTemplateCollectionModel(RequestTemplateCollection collection) {
        Name = new(() => collection.Name);
        DefaultUrl = new(() => collection.DefaultUrl);
    }

    public bool TryUpdateRequestTemplateCollection(RequestTemplateCollection collection) {
        if (!Name.Validate()) {
            return false;
        }

        collection.Name = Name;
        collection.DefaultUrl = DefaultUrl;
        return true;
    }

    public void Reset() {
        Name.Reset();
        DefaultUrl.Reset();
    }
}
