using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.Collections.Generic;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionModel : BoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly RequestTemplateCollectionFileModel file;
    private readonly RequestTemplateCollection collection;
    private readonly List<RequestTemplateModel> requests = [];

    public RequestTemplateCollectionSettingsModel Settings { get; }
    public NewRequestTemplateModel NewRequest { get; }
    public IReadOnlyList<RequestTemplateModel> Requests => requests;

    public RequestTemplateCollectionModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection
    ) : base(file.Name, file.Name) {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.popupService = popupService;
        this.messageService = messageService;
        this.file = file;
        this.collection = collection;

        Settings = new(requestTemplateCollectionService, messageService, file, collection);
        NewRequest = new(requestTemplateCollectionService, messageService, file, collection);

        foreach (var request in collection.Requests) {
            AddRequest(request);
        }
    }

    public RequestTemplateModel AddRequest(RequestTemplate request) {
        var model = new RequestTemplateModel(requestTemplateCollectionService, popupService, messageService, file, collection, request);
        requests.Add(model);
        return model;
    }


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
