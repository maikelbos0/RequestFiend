using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using RequestFiend.Models.Services;
using RequestFiend.UI.Views;
using System.IO;
using System.Threading.Tasks;

namespace RequestFiend.UI.Services;

public class PopupService : IPopupService {
    public async Task<bool> ShowConfirmPopup(string message) {
        var result = await Shell.Current.ShowPopupAsync<bool>(new ConfirmPopup(message));

        return !result.WasDismissedByTappingOutsideOfPopup && result.Result;
    }

    public Task ShowErrorPopup(string message)
        => Shell.Current.ShowPopupAsync(new ErrorPopup(message));

    public Task<FileSaverResult> ShowSaveDialog(string fileName, Stream stream) 
        => FileSaver.Default.SaveAsync(fileName, stream);
}
