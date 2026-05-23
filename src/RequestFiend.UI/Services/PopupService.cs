using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using RequestFiend.Core;
using RequestFiend.Models.Services;
using RequestFiend.UI.Views;
using System.IO;
using System.Threading.Tasks;
#if ANDROID
using Android.Content;
using Android.Provider;
using System;
#endif

namespace RequestFiend.UI.Services;

public class PopupService : IPopupService {
    public async Task<bool> ShowConfirmPopup(string message) {
        var result = await Shell.Current.ShowPopupAsync<bool>(new ConfirmPopup(message));

        return !result.WasDismissedByTappingOutsideOfPopup && result.Result;
    }

    public Task ShowErrorPopup(string message)
        => Shell.Current.ShowPopupAsync(new ErrorPopup(message));

    public Task<FileSaverResult> ShowSaveDialog(string fileName, Stream stream) {
        if (RequestFileAccess()) {
            return FileSaver.Default.SaveAsync(fileName, stream);
        }

        return Task.FromResult(new FileSaverResult(null, null));
    }

    public Task<FileResult?> ShowPickFileDialog(PickOptions pickOptions) {
        if (RequestFileAccess()) {
            return FilePicker.Default.PickAsync(pickOptions);
        }

        return Task.FromResult<FileResult?>(null);
    }

    private static bool RequestFileAccess() {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(30) && !Android.OS.Environment.IsExternalStorageManager) {
            Intent intent = new Intent(Settings.ActionManageAllFilesAccessPermission);

            intent.AddFlags(ActivityFlags.NewTask);

            Android.App.Application.Context.StartActivity(intent);

            return Android.OS.Environment.IsExternalStorageManager;
        }
#endif
        return true;
    }

    public Task<IPopupResult<string>> ShowUrlPopup(RequestTemplateCollection collection, string url)
        => Shell.Current.ShowPopupAsync<string>(new UrlPopup(collection, url));
}
