using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionModel : BoundModelBase {
    private readonly List<BoundModelBase> models = [];

    public RequestTemplateCollectionModel(RequestTemplateCollectionFileModel file) : base(file.Name, file.Name) { }

    public void AddChild(object child) {
        if (child is BoundModelBase model) {
            models.Add(model);
            model.PropertyChanged += OnChildStateChanged;
            UpdateState();
        }
    }

    public void RemoveChild(object child) {
        if (child is BoundModelBase model) {
            models.Remove(model);
            model.PropertyChanged -= OnChildStateChanged;
            UpdateState();
        }
    }

    private void OnChildStateChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == Constants.IsModified || e.PropertyName == Constants.HasError) {
            UpdateState();
        }
    }

    private void UpdateState() {
        var hasError = HasError;
        var isModified = IsModified;

        if (models.Any(model => model.HasError)) {
            HasError = true;
            IsModified = false;
        }
        else {
            HasError = false;
            IsModified = models.Any(model => model.IsModified);
        }

        if (hasError != HasError || isModified != IsModified) {
            UpdateTitles();
        }
    }
}
