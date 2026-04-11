using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Storage;
using RequestFiend.Core;
using System.IO;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IPopupService {
    Task<bool> ShowConfirmPopup(string message);
    Task ShowErrorPopup(string message);
    Task<FileSaverResult> ShowSaveDialog(string fileName, Stream stream);
    Task<FileResult?> ShowPickFileDialog(PickOptions pickOptions);
    Task<IPopupResult<string>> ShowUrlPopup(RequestTemplateCollection collection, string url);
}
