using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.Linq;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionModel : BoundModelBase {
    //private readonly List<BoundModelBase> models = [];

    public RequestTemplateCollectionSettingsModel Settings { get; }
    public NewRequestTemplateModel NewRequest { get; }
    public List<RequestTemplateModel> Requests { get; }

    public RequestTemplateCollectionModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection
    ) : base(file.Name, file.Name) {
        Settings = new(requestTemplateCollectionService, messageService, file, collection);
        NewRequest = new(requestTemplateCollectionService, messageService, file, collection);
        Requests = [.. collection.Requests.Select(request => new RequestTemplateModel(requestTemplateCollectionService, popupService, messageService, file, collection, request))];
    }


    //public void AddChild(object child) {
    //    if (child is BoundModelBase model) {
    //        models.Add(model);
    //        model.PropertyChanged += OnChildStateChanged;
    //        UpdateState();
    //    }
    //}

    //public void RemoveChild(object child) {
    //    if (child is BoundModelBase model) {
    //        models.Remove(model);
    //        model.PropertyChanged -= OnChildStateChanged;
    //        UpdateState();
    //    }
    //}

    //private void OnChildStateChanged(object? sender, PropertyChangedEventArgs e) {
    //    if (e.PropertyName == Constants.IsModified || e.PropertyName == Constants.HasError) {
    //        UpdateState();
    //    }
    //}

    //private void UpdateState() {
    //    var hasError = HasError;
    //    var isModified = IsModified;

    //    if (models.Any(model => model.HasError)) {
    //        HasError = true;
    //        IsModified = false;
    //    }
    //    else {
    //        HasError = false;
    //        IsModified = models.Any(model => model.IsModified);
    //    }

    //    if (hasError != HasError || isModified != IsModified) {
    //        UpdateTitles();
    //    }
    //}
}
