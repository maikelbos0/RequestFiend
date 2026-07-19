using CommunityToolkit.Mvvm.ComponentModel;
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

    [ObservableProperty] public partial bool ShowAllowScriptEvaluation { get; set; }
    public FileModel File { get; }
    public RequestTemplateCollection Collection { get; }
    public ValidatableProperty<bool> AllowScriptEvaluation { get; }
    public ValidatableProperty<string> DefaultUrl { get; }
    public NameValuePairModelCollection Variables { get; }
    public NameValuePairModelCollection DefaultHeaders { get; }
    public ValidatableProperty<bool> IgnoreRemoteCertificateNotAvailable { get; }
    public ValidatableProperty<bool> IgnoreRemoteCertificateNameMismatch { get; }
    public ValidatableProperty<bool> IgnoreRemoteCertificateChainErrors { get; }
    public ValidatableImmutableCollection<RequestTemplateItemModel> Requests { get; }

    public RequestTemplateCollectionSettingsModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        IPreferencesService preferencesService,
        FileModel file,
        RequestTemplateCollection collection
    ) : base($"{file.Name} - Collection settings", "Collection settings") {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.popupService = popupService;
        this.messageService = messageService;
        this.preferencesService = preferencesService;

        File = file;
        Collection = collection;

        ShowAllowScriptEvaluation = preferencesService.GetScriptEvaluationMode() == ScriptEvaluationMode.CollectionScoped;
        AllowScriptEvaluation = new(() => preferencesService.GetCollectionAllowScriptEvaluation(file.FilePath), value => preferencesService.SetCollectionAllowScriptEvaluation(File.FilePath, value));
        DefaultUrl = new(() => collection.DefaultUrl, value => collection.DefaultUrl = value);
        IgnoreRemoteCertificateNotAvailable = new(() => collection.IgnoreRemoteCertificateNotAvailable, value => collection.IgnoreRemoteCertificateNotAvailable = value);
        IgnoreRemoteCertificateNameMismatch = new(() => collection.IgnoreRemoteCertificateNameMismatch, value => collection.IgnoreRemoteCertificateNameMismatch = value);
        IgnoreRemoteCertificateChainErrors = new(() => collection.IgnoreRemoteCertificateChainErrors, value => collection.IgnoreRemoteCertificateChainErrors = value);
        Variables = new(collection.Variables, Validator.VariableName);
        DefaultHeaders = new(collection.DefaultHeaders, Validator.Required);
        Requests = new(
            () => collection.Requests.Select(request => new RequestTemplateItemModel(request)),
            value => {
                var sortOrder = value.Select((request, index) => new { request.Request, Index = index }).ToDictionary(x => x.Request, x => x.Index);
                Collection.Requests = [.. Collection.Requests.OrderBy(r => sortOrder.TryGetValue(r, out var order) ? order : int.MaxValue)];
            }
        );

        ConfigureState([AllowScriptEvaluation, DefaultUrl, IgnoreRemoteCertificateNotAvailable, IgnoreRemoteCertificateNameMismatch, IgnoreRemoteCertificateChainErrors, Variables, DefaultHeaders, Requests]);

        messageService.Register<RequestTemplateCollectionSettingsModel, PreferencesUpdatedMessage>(this, (_, _) => {
            ShowAllowScriptEvaluation = preferencesService.GetScriptEvaluationMode() == ScriptEvaluationMode.CollectionScoped;
            AllowScriptEvaluation.Reset();
        });

        messageService.Register<RequestTemplateCollectionSettingsModel, RequestTemplateRemovedFromCollectionMessage, FileModel>(this, file, (_, message) => {
            Requests.Remove(new(message.Request));
            Requests.Reset();
        });

        messageService.Register<RequestTemplateCollectionSettingsModel, RequestTemplateAddedToCollectionMessage, FileModel>(this, file, (_, message) => {
            Requests.Add(new(message.Request));
            Requests.Reset();
        });
    }

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        Set();

        await requestTemplateCollectionService.Save(File.FilePath, Collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
        messageService.Send(new RequestTemplateCollectionSettingsUpdatedMessage(Collection));
    }

    [RelayCommand]
    public void MoveRequestUp(RequestTemplateItemModel request) {
        var index = Requests.IndexOf(request);

        if (index > 0) {
            Requests.Remove(request);
            Requests.Insert(index - 1, request);
        }
    }

    [RelayCommand]
    public void MoveRequestDown(RequestTemplateItemModel request) {
        var index = Requests.IndexOf(request);

        if (index < Requests.Count - 1) {
            Requests.Remove(request);
            Requests.Insert(index + 1, request);
        }
    }

    [RelayCommand]
    public void ToggleAllowScriptEvaluation()
        => AllowScriptEvaluation.Value = !AllowScriptEvaluation.Value;

    [RelayCommand]
    public void ToggleIgnoreRemoteCertificateNotAvailable()
        => IgnoreRemoteCertificateNotAvailable.Value = !IgnoreRemoteCertificateNotAvailable.Value;

    [RelayCommand]
    public void ToggleIgnoreRemoteCertificateNameMismatch()
        => IgnoreRemoteCertificateNameMismatch.Value = !IgnoreRemoteCertificateNameMismatch.Value;

    [RelayCommand]
    public void ToggleIgnoreRemoteCertificateChainErrors()
        => IgnoreRemoteCertificateChainErrors.Value = !IgnoreRemoteCertificateChainErrors.Value;

    [RelayCommand]
    public async Task ShowDefaultUrlPopup() {
        var result = await popupService.ShowUrlPopup(Collection, DefaultUrl.Value);

        if (result.Result != null) {
            DefaultUrl.Value = result.Result;
            messageService.Send(new ValidatablePropertyUpdatedMessage(DefaultUrl));
        }
    }
}
