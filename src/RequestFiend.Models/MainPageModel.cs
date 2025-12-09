using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class MainPageModel : BoundModelBase {
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly IRecentCollectionService recentCollectionService;

    public List<RecentCollectionModel> RecentCollections { 
        get => field;
        set => SetProperty(ref field, value);
    }

    public MainPageModel(IPopupService popupService, IMessageService messageService, IRecentCollectionService recentCollectionService) {
        this.popupService = popupService;
        this.messageService = messageService;
        this.recentCollectionService = recentCollectionService;

        RecentCollections = recentCollectionService.Get();
    }

    [RelayCommand]
    public async Task CreateNewCollection() {
        var collection = new RequestTemplateCollection();
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, collection);

        var saveResult = await popupService.ShowSaveDialog(".json", stream);

        if (saveResult.IsSuccessful) {
            messageService.Send(new OpenCollectionRequestMessage(saveResult.FilePath, collection));
            RecentCollections = recentCollectionService.Push(saveResult.FilePath);
        }
        else if (saveResult.Exception != null) {
            await popupService.ShowErrorPopup($"Failed to create collection: {saveResult.Exception.Message}");
        }
    }
}
