using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.Collections.ObjectModel;
using System.Linq;

namespace RequestFiend.Models;

public class RequestTemplateCollectionModel : BoundModelBase {
    public OptionalString DefaultUrl { get; set; }
    public ObservableCollection<HeaderTemplateModel> DefaultHeaders { get; set; }
    public ObservableCollection<VariableModel> Variables { get; set; }

    public RequestTemplateCollectionModel(RequestTemplateCollection collection) {
        DefaultUrl = new(() => collection.DefaultUrl);
        DefaultHeaders = new(collection.DefaultHeaders.Select(variable => new HeaderTemplateModel(variable)));
        Variables = new(collection.Variables.Select(header => new VariableModel(header)));
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
        collection.DefaultHeaders = DefaultHeaders.Select(header => new HeaderTemplate() {
            Name = header.Name,
            Value = header.Value
        }).ToList();
        collection.Variables = Variables.Select(variable => new Variable() {
            Name = variable.Name,
            Value = variable.Value,
        }).ToList();
        
        return true;
    }
}
