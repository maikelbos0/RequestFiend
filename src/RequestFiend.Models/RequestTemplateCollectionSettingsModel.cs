using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionSettingsModel : PageBoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly IPreferencesService preferencesService;

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

    // TODO make it a proper collection
    public ObservableCollection<RequestTemplate> Requests { get; }

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
        AllowScriptEvaluation = new(() => preferencesService.GetCollectionAllowScriptEvaluation(file.FilePath));
        DefaultUrl = new(() => collection.DefaultUrl, value => collection.DefaultUrl = value);
        IgnoreRemoteCertificateNotAvailable = new(() => collection.IgnoreRemoteCertificateNotAvailable, setter: value => collection.IgnoreRemoteCertificateNotAvailable = value);
        IgnoreRemoteCertificateNameMismatch = new(() => collection.IgnoreRemoteCertificateNameMismatch, setter: value => collection.IgnoreRemoteCertificateNameMismatch = value);
        IgnoreRemoteCertificateChainErrors = new(() => collection.IgnoreRemoteCertificateChainErrors, setter: value => collection.IgnoreRemoteCertificateChainErrors = value);
        Variables = new(collection.Variables, Validator.VariableName);
        DefaultHeaders = new(collection.DefaultHeaders, Validator.Required);
        Requests = new(collection.Requests);

        ConfigureState([AllowScriptEvaluation, DefaultUrl, IgnoreRemoteCertificateNotAvailable, IgnoreRemoteCertificateNameMismatch, IgnoreRemoteCertificateChainErrors, Variables, DefaultHeaders]);
        messageService.Register<RequestTemplateCollectionSettingsModel, PreferencesUpdatedMessage>(this, (_, _) => {
            ShowAllowScriptEvaluation = preferencesService.GetScriptEvaluationMode() == ScriptEvaluationMode.CollectionScoped;
            AllowScriptEvaluation.Reset();
        });
    }

    // TODO receive request for request added

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        var sortOrder = Requests.Select((requestTemplate, index) => (requestTemplate, index)).ToDictionary(x => x.requestTemplate, x => x.index);
        Collection.Requests = [.. Collection.Requests.OrderBy(r => sortOrder.TryGetValue(r, out var order) ? order : int.MaxValue)];
        preferencesService.SetCollectionAllowScriptEvaluation(File.FilePath, AllowScriptEvaluation.Value);
        Reset();

        await requestTemplateCollectionService.Save(File.FilePath, Collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
        messageService.Send(new RequestTemplateCollectionSettingsUpdatedMessage(Collection));
    }

    [RelayCommand]
    public void MoveRequestUp(RequestTemplate request) {
        var index = Requests.IndexOf(request);

        if (index > 0) {
            Requests.Remove(request);
            Requests.Insert(index - 1, request);
        }
    }

    [RelayCommand]
    public void MoveRequestDown(RequestTemplate request) {
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
