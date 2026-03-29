using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Linq;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionSettingsModel : PageBoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly RequestTemplateCollectionFileModel file;
    private readonly RequestTemplateCollection collection;

    public ValidatableProperty<string> DefaultUrl { get; }
    public NameValuePairModelCollection DefaultHeaders { get; }
    public NameValuePairModelCollection Variables { get; }

    public RequestTemplateCollectionSettingsModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection
    ) : base($"{file.Name} - Collection settings", "Collection settings") {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.popupService = popupService;
        this.messageService = messageService;
        this.file = file;
        this.collection = collection;

        DefaultUrl = new(() => collection.DefaultUrl);
        DefaultHeaders = new(collection.DefaultHeaders, Validator.Required);
        Variables = new(collection.Variables, Validator.VariableName);

        ConfigureState([DefaultUrl, DefaultHeaders, Variables]);
    }

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        collection.DefaultUrl = DefaultUrl.Value;
        collection.DefaultHeaders = [.. DefaultHeaders.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        collection.Variables = [.. Variables.Select(variable => new NameValuePair() { Name = variable.Name.Value!, Value = variable.Value.Value!, })];

        DefaultUrl.Reset();
        DefaultHeaders.Reset(collection.DefaultHeaders);
        Variables.Reset(collection.Variables);

        await requestTemplateCollectionService.Save(file.FilePath, collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
    }

    [RelayCommand]
    public async Task ShowDefaultUrlPopup() {
        var result = await popupService.ShowUrlPopup(DefaultUrl.Value);

        if (result.Result != null) {
            DefaultUrl.Value = result.Result;
        }
    }
}
