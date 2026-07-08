using RequestFiend.Core;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.Linq;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionModel : PageBoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly IEnvironmentService environmentService;
    private readonly FileModel file;
    private readonly RequestTemplateCollection collection;
    private readonly List<RequestTemplateModel> requests = [];

    public RequestTemplateCollectionSettingsModel Settings { get; }
    public NewRequestTemplateModel NewRequest { get; }
    public IEnumerable<RequestTemplateModel> Requests => requests;

    public RequestTemplateCollectionModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        IPreferencesService preferencesService,
        IEnvironmentService environmentService,
        FileModel file,
        RequestTemplateCollection collection
    ) : base(file.Name, file.Name) {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.popupService = popupService;
        this.messageService = messageService;
        this.environmentService = environmentService;
        this.file = file;
        this.collection = collection;

        Settings = new(requestTemplateCollectionService, popupService, messageService, preferencesService, file, collection);
        NewRequest = new(requestTemplateCollectionService, popupService, messageService, file, collection);
        requests.AddRange(collection.Requests.Select(request => new RequestTemplateModel(requestTemplateCollectionService, popupService, messageService, environmentService, file, collection, request)));

        ConfigureState([Settings, NewRequest, .. requests]);
    }

    public RequestTemplateModel AddRequest(RequestTemplate request) {
        var model = new RequestTemplateModel(requestTemplateCollectionService, popupService, messageService, environmentService, file, collection, request);
        requests.Add(model);
        ConfigureState([model]);
        return model;
    }
}
