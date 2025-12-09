using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.IO;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class NewRequestTemplateModel : BoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IMessageService messageService;
    private readonly string filePath;
    private readonly RequestTemplateCollection collection;

    public string Title { get => field; set => SetProperty(ref field, value); }
    public ValidatableString Name { get; set; } = new(true);
    public ValidatableString Method { get; set; } = new(true);
    public ValidatableString Url { get; set; }

    public NewRequestTemplateModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IMessageService messageService,
        IModelDataProvider<(string, RequestTemplateCollection)> modelDataProvider
    ) {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.messageService = messageService;
        (filePath, collection) = modelDataProvider.GetData();

        Title = $"{Path.GetFileNameWithoutExtension(filePath)} - New request";
        Url = new(true, () => collection.DefaultUrl);
        messageService.Register<NewRequestTemplateModel, RequestTemplateCollectionUpdatedMessage, string>(this, filePath, (model, _) => {
            if (!model.Url.IsModified) {
                model.Url.Reset();
            }
        });
    }

    [RelayCommand]
    public async Task Create() {
        if (Name.HasError || Method.HasError || Url.HasError) {
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

        await requestTemplateCollectionService.Save(filePath, collection);
        messageService.Send(new OpenTemplateRequestMessage(filePath, collection, request));
        messageService.Send(new SuccessMessage("Request had been added"));
    }
}
