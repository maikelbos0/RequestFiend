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
    private readonly IPreferencesService preferencesService;

    [ObservableProperty] public partial bool ShowAllowScriptEvaluation { get; set; }
    public RequestTemplateCollectionFileModel File { get; }
    public RequestTemplateCollection Collection { get; }
    public ValidatableProperty<bool> AllowScriptEvaluation { get; set; }
    public ValidatableProperty<string> DefaultUrl { get; }
    public NameValuePairModelCollection Variables { get; }
    public NameValuePairModelCollection DefaultHeaders { get; }
    public ValidatableProperty<bool> IgnoreRemoteCertificateNotAvailable { get; }
    public ValidatableProperty<bool> IgnoreRemoteCertificateNameMismatch { get; }
    public ValidatableProperty<bool> IgnoreRemoteCertificateChainErrors { get; }

    public RequestTemplateCollectionSettingsModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        IPreferencesService preferencesService,
        RequestTemplateCollectionFileModel file,
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
        DefaultUrl = new(() => collection.DefaultUrl);
        IgnoreRemoteCertificateNotAvailable = new(() => collection.IgnoreRemoteCertificateNotAvailable);
        IgnoreRemoteCertificateNameMismatch = new(() => collection.IgnoreRemoteCertificateNameMismatch);
        IgnoreRemoteCertificateChainErrors = new(() => collection.IgnoreRemoteCertificateChainErrors);
        Variables = new(collection.Variables, Validator.VariableName);
        DefaultHeaders = new(collection.DefaultHeaders, Validator.Required);

        ConfigureState([AllowScriptEvaluation, DefaultUrl, IgnoreRemoteCertificateNotAvailable, IgnoreRemoteCertificateNameMismatch, IgnoreRemoteCertificateChainErrors, DefaultHeaders, Variables]);
        messageService.Register<RequestTemplateCollectionSettingsModel, PreferencesUpdatedMessage>(this, (_, _) => {
            ShowAllowScriptEvaluation = preferencesService.GetScriptEvaluationMode() == ScriptEvaluationMode.CollectionScoped;
            AllowScriptEvaluation.Reset();
        });
    }

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        preferencesService.SetCollectionAllowScriptEvaluation(File.FilePath, AllowScriptEvaluation.Value);
        Collection.DefaultUrl = DefaultUrl.Value;
        Collection.Variables = [.. Variables.Select(variable => new NameValuePair() { Name = variable.Name.Value!, Value = variable.Value.Value!, })];
        Collection.DefaultHeaders = [.. DefaultHeaders.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        Collection.IgnoreRemoteCertificateNotAvailable = IgnoreRemoteCertificateNotAvailable.Value;
        Collection.IgnoreRemoteCertificateNameMismatch = IgnoreRemoteCertificateNameMismatch.Value;
        Collection.IgnoreRemoteCertificateChainErrors = IgnoreRemoteCertificateChainErrors.Value;

        AllowScriptEvaluation.Reset();
        DefaultUrl.Reset();
        IgnoreRemoteCertificateNotAvailable.Reset();
        IgnoreRemoteCertificateNameMismatch.Reset();
        IgnoreRemoteCertificateChainErrors.Reset();
        Variables.Reset(Collection.Variables);
        DefaultHeaders.Reset(Collection.DefaultHeaders);

        await requestTemplateCollectionService.Save(File.FilePath, Collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
        messageService.Send(new RequestTemplateCollectionSettingsUpdatedMessage(Collection));
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
