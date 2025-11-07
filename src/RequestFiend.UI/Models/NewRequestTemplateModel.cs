using RequestFiend.Core;
using RequestFiend.UI.Models.Validation;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RequestFiend.UI.Models;

public class NewRequestTemplateModel {
    private readonly RequestTemplateCollection collection;

    // TODO can this move to static data?
    public List<string> Methods { get; set; } = ["GET", "PUT", "POST", "DELETE", "HEAD", "OPTIONS", "TRACE", "PATCH"];
    public RequiredString Name { get; set; } = new();
    public RequiredString Method { get; set; } = new();
    public RequiredString Url { get; set; } = new();

    public NewRequestTemplateModel(RequestTemplateCollection collection) {
        this.collection = collection;
        Url.Reset(collection.DefaultUrl);
    }

    public bool TryCreateRequestTemplate([NotNullWhen(true)] out RequestTemplate? request) {
        if (!Name.Validate() | !Method.Validate() | !Url.Validate()) {
            request = null;
            return false;
        }

        request = new() {
            Name = Name,
            Method = Method,
            Url = Url
        };
        return true;
    }

    public void Reset() {
        Name.Reset();
        Method.Reset(); 
        Url.Reset(collection.DefaultUrl);
    }
}
