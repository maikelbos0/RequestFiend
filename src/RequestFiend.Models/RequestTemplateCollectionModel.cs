using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.Linq;

namespace RequestFiend.Models;

public class RequestTemplateCollectionModel : BoundModelBase {
    public OptionalString DefaultUrl { get; set; }
    public NameValuePairModelCollection DefaultHeaders { get; set; }
    public NameValuePairModelCollection Variables { get; set; }

    public RequestTemplateCollectionModel(RequestTemplateCollection collection) {
        DefaultUrl = new(() => collection.DefaultUrl);
        DefaultHeaders = [.. collection.DefaultHeaders.Select(variable => new NameValuePairModel(variable))];
        Variables = [.. collection.Variables.Select(header => new NameValuePairModel(header))];
    }

    public bool TryUpdateRequestTemplateCollection(RequestTemplateCollection collection) {
        var isValid = true;

        foreach (var header in DefaultHeaders) {
            isValid = isValid & header.Name.Validate() & header.Value.Validate();
        }

        foreach (var variable in Variables) {
            isValid = isValid & variable.Name.Validate() & variable.Value.Validate();
        }

        if (!isValid) {
            return false;
        }

        collection.DefaultUrl = DefaultUrl;
        collection.DefaultHeaders = [.. DefaultHeaders.Select(header => new NameValuePair() { Name = header.Name, Value = header.Value })];
        collection.Variables = [.. Variables.Select(variable => new NameValuePair() { Name = variable.Name, Value = variable.Value, })];

        return true;
    }
}
