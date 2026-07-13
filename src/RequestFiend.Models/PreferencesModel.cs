using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class PreferencesModel : PageBoundModelBase {
    private readonly IPreferencesService preferencesService;
    private readonly IMessageService messageService;
    private readonly IPopupService popupService;
    private readonly IFileSystem fileSystem;
    private readonly IEnvironmentService environmentService;

    public ValidatableProperty<string> MaximumRecentCollectionCount { get; }
    public ValidatableProperty<string> ScriptEvaluationMode { get; }
    public ValidatableProperty<string> RequestTimeoutInSeconds { get; }
    public ValidatableProperty<string> ExchangeLoggingPath { get; }
    public ValidatableProperty<string> ExchangeLoggingOutputTemplate { get; }
    public ValidatableImmutableCollection<FileModel> Environments { get; }
    public ValidatableProperty<FileModel?> ActiveEnvironment { get; }

    public PreferencesModel(IPreferencesService preferencesService, IMessageService messageService, IPopupService popupService, IFileSystem fileSystem, IEnvironmentService environmentService) : base("Preferences", "Preferences") {
        this.preferencesService = preferencesService;
        this.messageService = messageService;
        this.popupService = popupService;
        this.fileSystem = fileSystem;
        this.environmentService = environmentService;
        MaximumRecentCollectionCount = new(() => preferencesService.GetMaximumRecentCollectionCount().ToString(), Validator.Numeric);
        ScriptEvaluationMode = new(() => Options.ScriptEvaluationModeMap[preferencesService.GetScriptEvaluationMode()]);
        RequestTimeoutInSeconds = new(() => preferencesService.GetRequestTimeoutInSeconds()?.ToString() ?? "", Validator.Numeric);
        ExchangeLoggingPath = new(preferencesService.GetExchangeLoggingPath);
        ExchangeLoggingOutputTemplate = new(preferencesService.GetExchangeLoggingOutputTemplate);
        Environments = new(preferencesService.GetEnvironments().Distinct().OrderBy(environment => environment.Name, System.StringComparer.CurrentCultureIgnoreCase));
        ActiveEnvironment = new(() => Environments.SingleOrDefault(environment => environment == preferencesService.GetActiveEnvironment()));

        ConfigureState([RequestTimeoutInSeconds, MaximumRecentCollectionCount, ScriptEvaluationMode, ExchangeLoggingPath, ExchangeLoggingOutputTemplate, Environments, ActiveEnvironment]);
    }

    [RelayCommand]
    public void Update() {
        if (HasError) {
            return;
        }

        preferencesService.SetMaximumRecentCollectionCount(int.Parse("0" + MaximumRecentCollectionCount.Value));
        preferencesService.SetScriptEvaluationMode(GetScriptEvaluationMode());
        preferencesService.SetRequestTimeoutInSeconds(RequestTimeoutInSeconds.Value.Length == 0 ? null : int.Parse(RequestTimeoutInSeconds.Value));
        preferencesService.SetExchangeLoggingPath(ExchangeLoggingPath.Value);
        preferencesService.SetExchangeLoggingOutputTemplate(ExchangeLoggingOutputTemplate.Value);
        preferencesService.SetEnvironments(Environments);
        preferencesService.SetActiveEnvironment(ActiveEnvironment.Value);

        preferencesService.TrimRecentCollections();

        Reset();

        messageService.Send(new PreferencesUpdatedMessage());
        messageService.Send(new SuccessMessage("Preferences have been updated"));
    }

    private ScriptEvaluationMode GetScriptEvaluationMode() {
        if (Options.ReverseScriptEvaluationModeMap.TryGetValue(ScriptEvaluationMode.Value, out var scriptEvaluationMode)) {
            return scriptEvaluationMode;
        }

        return Models.ScriptEvaluationMode.Disabled;
    }

    [RelayCommand]
    public async Task CreateNewEnvironment() {
        var environment = new Environment();
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, environment);

        var saveResult = await popupService.ShowSaveDialog(".json", stream);

        if (saveResult.IsSuccessful) {
            messageService.Send(new SuccessMessage("Environment has been created"));
            AddEnvironment(new(saveResult.FilePath));
            await popupService.ShowEnvironmentPopup(environmentService, new(saveResult.FilePath), environment);
        }
        else if (saveResult.Exception != null && saveResult.Exception is not System.OperationCanceledException) {
            await popupService.ShowErrorPopup($"Failed to create collection: {saveResult.Exception.Message}");
        }
    }

    [RelayCommand]
    public async Task OpenExistingEnvironment() {
        var file = await popupService.ShowPickFileDialog(new() {
            FileTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>() {
                { DevicePlatform.Android, ["application/json"] },
                { DevicePlatform.iOS, ["public.json"] },
                { DevicePlatform.MacCatalyst, ["public.json"] },
                { DevicePlatform.WinUI, ["*.json"] },
            })
        });

        if (file != null) {
            AddEnvironment(new(file.FullPath));
        }
    }

    private void AddEnvironment(FileModel newEnvironment) {
        if (!Environments.Contains(newEnvironment)) {
            var index = Environments
                .Select((environment, index) => new { Index = index, Comparison = System.StringComparer.InvariantCultureIgnoreCase.Compare(environment.Name, newEnvironment.Name) })
                .Where(item => item.Comparison < 0)
                .Select(item => item.Index + 1)
                .DefaultIfEmpty(0)
                .Max();

            Environments.Insert(index, newEnvironment);
            ActiveEnvironment.Value = newEnvironment;
        }
    }

    [RelayCommand]
    public void RemoveEnvironment(FileModel environment) {
        if (environment == ActiveEnvironment.Value) {
            ActiveEnvironment.Value = null;
        }
        Environments.Remove(environment);
    }

    [RelayCommand]
    public async Task OpenEnvironmentPopup(FileModel file) {
        if (fileSystem.File.Exists(file.FilePath)) {
            try {
                var environment = JsonSerializer.Deserialize<Environment>(await fileSystem.File.ReadAllTextAsync(file.FilePath));

                if (environment != null) {
                    await popupService.ShowEnvironmentPopup(environmentService, file, environment);
                }
                else {
                    await popupService.ShowErrorPopup("Failed to load environment.");
                }
            }
            catch (System.Exception exception) {
                await popupService.ShowErrorPopup($"Failed to load environment: {exception.Message}");
            }
        }
        else {
            await popupService.ShowErrorPopup("Environment file does not exist.");
        }
    }

    [RelayCommand]
    public async Task ResetPreferences() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to reset your preferences?")) {
            preferencesService.Reset();

            Environments.Clear();
            Reset();

            messageService.Send(new PreferencesUpdatedMessage());
            messageService.Send(new SuccessMessage("Preferences have been reset"));
        }
    }

    [RelayCommand]
    public async Task ClearRecentCollections() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to clear your recent collections?")) {
            preferencesService.ClearRecentCollections();

            messageService.Send(new SuccessMessage("Recent collections have been cleared"));
        }
    }
}
