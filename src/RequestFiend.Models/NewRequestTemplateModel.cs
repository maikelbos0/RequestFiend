using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class NewRequestTemplateModel : PageBoundModelBase, IVariableSnapshotProvider {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly IEnvironmentService environmentService;

    public FileModel File { get; }

    // TODO private
    public RequestTemplateCollection Collection { get; }

    public ValidatableProperty<string> Name { get; } = new(() => "", _ => { }, Validator.Required);
    public ValidatableProperty<string> Method { get; } = new(() => "GET", _ => { }, Validator.Required);
    public ValidatableProperty<string> Url { get; }
    
    public NewRequestTemplateModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        IEnvironmentService environmentService,
        FileModel file,
        RequestTemplateCollection collection
    ) : base($"{file.Name} - New request", "New request") {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.popupService = popupService;
        this.messageService = messageService;
        this.environmentService = environmentService;
        File = file;        
        Collection = collection;

        Url = new(() => collection.DefaultUrl, _ => { }, Validator.Required);
        messageService.Register<NewRequestTemplateModel, RequestTemplateCollectionUpdatedMessage, FileModel>(this, file, (model, _) => {
            if (!model.Url.IsModified) {
                model.Url.Reset();
            }
        });

        ConfigureState([Name, Method, Url]);
    }

    [RelayCommand]
    public async Task Create() {
        if (HasError) {
            return;
        }

        var request = new RequestTemplate() {
            Name = Name.Value,
            Method = Method.Value,
            Url = Url.Value
        };
        Collection.Requests.Add(request);

        Reset();

        await requestTemplateCollectionService.Save(File, Collection);
        messageService.Send(new RequestTemplateCreatedMessage(File.FilePath, Collection, request));
        messageService.Send(new RequestTemplateAddedToCollectionMessage(request), File);
        messageService.Send(new SuccessMessage("Request has been added"));
    }

    [RelayCommand]
    public async Task ShowUrlPopup() {
        var result = await popupService.ShowUrlPopup(environmentService, Collection, Url.Value);

        if (result.Result != null) {
            Url.Value = result.Result;
            messageService.Send(new ValidatablePropertyUpdatedMessage(Url));
        }
    }

    public async Task<VariableSnapshot> CreateVariableSnapshot()
        => Collection.CreateVariableSnapshot(await environmentService.GetActiveEnvironment());
}
