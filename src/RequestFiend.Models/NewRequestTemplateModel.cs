using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class NewRequestTemplateModel : PageBoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IMessageService messageService;

    public RequestTemplateCollectionFileModel File { get; }
    public RequestTemplateCollection Collection { get; }
    public ValidatableProperty<string> Name { get; } = new(() => "", Validator.Required);
    public ValidatableProperty<string> Method { get; } = new(() => "GET", Validator.Required);
    public ValidatableProperty<string> Url { get; }
    
    public NewRequestTemplateModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IMessageService messageService,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection
    ) : base($"{file.Name} - New request", "New request") {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.messageService = messageService;

        File = file;        
        Collection = collection;

        Url = new(() => collection.DefaultUrl, Validator.Required);
        messageService.Register<NewRequestTemplateModel, RequestTemplateCollectionUpdatedMessage, string>(this, file.FilePath, (model, _) => {
            if (!model.Url.IsModified) {
                model.Url.Reset();
            }
        });

        ConfigureState([Name, Method, Url]);
        WeakReferenceMessenger.Default.RegisterAll(this, file);
    }

    [RelayCommand]
    public async Task Create() {
        if (HasError) {
            return;
        }

        var request = new RequestTemplate() {
            Name = Name.Value!,
            Method = Method.Value!,
            Url = Url.Value!
        };
        Collection.Requests.Add(request);

        Name.Reset();
        Method.Reset();
        Url.Reset();

        await requestTemplateCollectionService.Save(File.FilePath, Collection);
        messageService.Send(new OpenTemplateRequestMessage(File.FilePath, Collection, request));
        messageService.Send(new SuccessMessage("Request had been added"));
    }
}
