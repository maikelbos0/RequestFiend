using RequestFiend.Core;
using RequestFiend.UI.Models.Validation;
using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.UI.Models;

public class NewRequestTemplateCollectionModel {
    public RequiredString Name { get; set; } = new();
    public string? DefaultUrl { get; set; }

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
        DefaultUrl = null;
    }
}

