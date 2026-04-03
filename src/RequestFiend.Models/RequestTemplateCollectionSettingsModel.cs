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

    public RequestTemplateCollectionFileModel File { get; }
    public RequestTemplateCollection Collection { get; }
    public ValidatableProperty<string> DefaultUrl { get; }
    public NameValuePairModelCollection Variables { get; }
    public NameValuePairModelCollection DefaultHeaders { get; }

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

        File = file;
        Collection = collection;

        DefaultUrl = new(() => collection.DefaultUrl);
        Variables = new(collection.Variables, Validator.VariableName);
        DefaultHeaders = new(collection.DefaultHeaders, Validator.Required);

        ConfigureState([DefaultUrl, DefaultHeaders, Variables]);
    }

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        Collection.DefaultUrl = DefaultUrl.Value;
        Collection.Variables = [.. Variables.Select(variable => new NameValuePair() { Name = variable.Name.Value!, Value = variable.Value.Value!, })];
        Collection.DefaultHeaders = [.. DefaultHeaders.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];

        DefaultUrl.Reset();
        Variables.Reset(Collection.Variables);
        DefaultHeaders.Reset(Collection.DefaultHeaders);

        await requestTemplateCollectionService.Save(File.FilePath, Collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
        messageService.Send(new RequestTemplateCollectionSettingsUpdatedMessage(Collection));
    }

    [RelayCommand]
    public async Task ShowDefaultUrlPopup() {
        var result = await popupService.ShowUrlPopup(DefaultUrl.Value);

        if (result.Result != null) {
            DefaultUrl.Value = result.Result;
        }
    }
}
