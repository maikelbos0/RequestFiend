using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.Models;

public class NewRequestTemplateModel : RequestTemplateCollectionModelBase {
    public ValidatableString Name { get; set; } = new(true);
    public ValidatableString Method { get; set; } = new(true);
    public ValidatableString Url { get; set; } 

    public NewRequestTemplateModel(IFileService fileService, string filePath, RequestTemplateCollection collection) : base(fileService, filePath, collection) {
        Url = new(true, () => collection.DefaultUrl);
    }

    public bool TryCreateRequestTemplate([NotNullWhen(true)] out RequestTemplate? request) {
        if (Name.HasError || Method.HasError || Url.HasError) {
            request = null;
            return false;
        }

        request = new() {
            Name = Name.Value!,
            Method = Method.Value!,
            Url = Url.Value!
        };
        return true;
    }

    public void Reset() {
        Name.Reset();
        Method.Reset(); 
        Url.Reset();
    }
}
