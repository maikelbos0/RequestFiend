using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class NewRequestTemplateModel : BoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IMessageService messageService;
    private readonly RequestTemplateCollectionFileModel file;
    private readonly RequestTemplateCollection collection;

    public ValidatableProperty<string> Name { get; set; } = new(() => "", Validator.Required);
    public ValidatableProperty<string> Method { get; set; } = new(() => "", Validator.Required);
    public ValidatableProperty<string> Url { get; set; }

    public NewRequestTemplateModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IMessageService messageService,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection
    ) : base($"{file.Name} - New request", "New request") {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.messageService = messageService;
        this.file = file;
        this.collection = collection;

        Url = new(() => collection.DefaultUrl, Validator.Required);
        messageService.Register<NewRequestTemplateModel, RequestTemplateCollectionUpdatedMessage, string>(this, file.FilePath, (model, _) => {
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
            Name = Name.Value!,
            Method = Method.Value!,
            Url = Url.Value!
        };
        collection.Requests.Add(request);

        Name.Reset();
        Method.Reset();
        Url.Reset();

        await requestTemplateCollectionService.Save(file.FilePath, collection);
        messageService.Send(new OpenTemplateRequestMessage(file.FilePath, collection, request));
        messageService.Send(new SuccessMessage("Request had been added"));
    }
}
