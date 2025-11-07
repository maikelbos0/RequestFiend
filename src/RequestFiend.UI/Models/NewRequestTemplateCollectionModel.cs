using RequestFiend.Core;
using RequestFiend.UI.Models.Properties;
using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.UI.Models;

public class NewRequestTemplateCollectionModel {
    public RequiredString Name { get; set; } = new();
    public OptionalString DefaultUrl { get; set; } = new();

    public bool TryCreateRequestTemplateCollection([NotNullWhen(true)] out RequestTemplateCollection? collection) {
        if (!Name.Validate()) {
            collection = null;
            return false;
        }

        collection = new() {
            Name = Name,
            DefaultUrl = DefaultUrl
        };
        return true;
    }

    public void Reset() {
        Name.Reset();
        DefaultUrl.Reset();
    }
}

