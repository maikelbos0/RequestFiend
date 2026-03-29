using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionModel : PageBoundModelBase {
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

        Settings = new(requestTemplateCollectionService, popupService, messageService, file, collection);
        Settings.PropertyChanged += OnChildStateChanged;
        NewRequest = new(requestTemplateCollectionService, messageService, file, collection);
        NewRequest.PropertyChanged += OnChildStateChanged;

        foreach (var request in collection.Requests) {
            AddRequest(request);
        }

        UpdateState();
    }

    public RequestTemplateModel AddRequest(RequestTemplate request) {
        var model = new RequestTemplateModel(requestTemplateCollectionService, popupService, messageService, file, collection, request);
        model.PropertyChanged += OnChildStateChanged;
        requests.Add(model);
        return model;
    }

    private void OnChildStateChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IsModified) || e.PropertyName == nameof(HasError)) {
            UpdateState();
        }
    }

    protected override void UpdateState() {
        var models = requests.Cast<PageBoundModelBase>().Append(Settings).Append(NewRequest);
        var isModified = IsModified;

        HasError = models.Any(model => model.HasError);
        IsModified = models.Any(model => model.IsModified);
        IsModifiedWithoutError = IsModified && !HasError;

        if (isModified != IsModified) {
            UpdateTitles();
        }
    }
}
