using CommunityToolkit.Maui.Storage;
using System.IO;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IPopupService {
    Task<bool> ShowConfirmPopup(string message);
    Task ShowErrorPopup(string message);
    Task<FileSaverResult> ShowSaveDialog(string fileName, Stream stream);
}
