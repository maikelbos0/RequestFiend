using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class MainPageModel : PageBoundModelBase {
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly IPreferencesService preferencesService;
    private readonly IFileSystem fileSystem;

    [ObservableProperty] public partial List<RequestTemplateCollectionFileModel> RecentCollections { get; set; }

    public MainPageModel(IPopupService popupService, IMessageService messageService, IPreferencesService preferencesService, IFileSystem fileSystem) : base("Home", "Home") {
        this.popupService = popupService;
        this.messageService = messageService;
        this.preferencesService = preferencesService;
        this.fileSystem = fileSystem;

        RecentCollections = preferencesService.GetRecentCollections();

        messageService.Register<MainPageModel, RecentCollectionsChangedMessage>(this, (model, _) => model.RecentCollections = preferencesService.GetRecentCollections());

        ConfigureState([]);
    }

    [RelayCommand]
    public async Task CreateNewCollection() {
        var collection = new RequestTemplateCollection();
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, collection);

        var saveResult = await popupService.ShowSaveDialog(".json", stream);

        if (saveResult.IsSuccessful) {
            messageService.Send(new OpenCollectionRequestMessage(saveResult.FilePath, collection));
            preferencesService.PushRecentCollection(saveResult.FilePath);
        }
        else if (saveResult.Exception != null && saveResult.Exception is not OperationCanceledException) {
            await popupService.ShowErrorPopup($"Failed to create collection: {saveResult.Exception.Message}");
        }
    }

    [RelayCommand]
    public async Task OpenExistingCollection() {
        var file = await popupService.ShowPickFileDialog(new() {
            FileTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>() {
                { DevicePlatform.Android, ["application/json"] },
                { DevicePlatform.iOS, ["public.json"] },
                { DevicePlatform.MacCatalyst, ["public.json"] },
                { DevicePlatform.WinUI, ["*.json"] },
            })
        });

        if (file != null) {
            await OpenCollection(file.FullPath);
        }
    }

    [RelayCommand]
    public async Task OpenCollection(string filePath) {
        if (fileSystem.File.Exists(filePath)) {
            try {
                var collection = JsonSerializer.Deserialize<RequestTemplateCollection>(await fileSystem.File.ReadAllTextAsync(filePath));

                if (collection != null) {
                    messageService.Send(new OpenCollectionRequestMessage(filePath, collection));
                    preferencesService.PushRecentCollection(filePath);
                }
                else {
                    await popupService.ShowErrorPopup("Failed to load collection.");
                }
            }
            catch (Exception exception) {
                await popupService.ShowErrorPopup($"Failed to load collection: {exception.Message}");
            }
        }
        else {
            await popupService.ShowErrorPopup("Collection file does not exist.");
            preferencesService.RemoveRecentCollection(filePath);
        }
    }
}
