using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public class EnvironmentService : IEnvironmentService, IRecipient<ActiveEnvironmentChangedMessage> {
    private readonly IFileSystem fileSystem;
    private readonly IPreferencesService preferencesService;
    private readonly IPopupService popupService;
    private Environment? activeEnvironment;

    public EnvironmentService(IFileSystem fileSystem, IPreferencesService preferencesService, IPopupService popupService) {
        this.fileSystem = fileSystem;
        this.preferencesService = preferencesService;
        this.popupService = popupService;
    }

    public Task Save(string filePath, Environment environment)
        => fileSystem.File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(environment));


    public async Task<Environment> GetActiveEnvironment() {
        if (activeEnvironment == null) {
            var filePath = preferencesService.GetActiveEnvironment()?.FilePath;

            if (filePath != null) {
                if (fileSystem.File.Exists(filePath)) {
                    try {
                        activeEnvironment = JsonSerializer.Deserialize<Environment>(await fileSystem.File.ReadAllTextAsync(filePath));
                    }
                    catch (System.Exception exception) {
                        await popupService.ShowErrorPopup($"Failed to load active environment: {exception.Message}");
                    }
                }
                else {
                    await popupService.ShowErrorPopup("Active environment file does not exist.");
                }
            }
        }

        return activeEnvironment ??= new();
    }

    public void Receive(ActiveEnvironmentChangedMessage _)
        => activeEnvironment = null;
}
