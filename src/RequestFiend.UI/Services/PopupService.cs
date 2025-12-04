using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using RequestFiend.Models.Services;
using RequestFiend.UI.Views;
using System.Threading.Tasks;

namespace RequestFiend.UI.Services;

public class PopupService : IPopupService {
    public async Task<bool> ShowConfirmPopup(string text) {
        var result = await Shell.Current.ShowPopupAsync<bool>(new ConfirmPopup(text));

        return !result.WasDismissedByTappingOutsideOfPopup && result.Result;
    }
}
