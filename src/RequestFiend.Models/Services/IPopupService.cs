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
    Task<IPopupResult<string>> ShowUrlPopup(IEnvironmentService environmentService, RequestTemplateCollection collection, string url);
    Task<IPopupResult> ShowEnvironmentPopup(IEnvironmentService environmentService, IPopupService popupService, ISecretEncryptor secretEncryptor, FileModel file, Environment environment);
    Task<IPopupResult<bool>> ShowUnlockPopup(ISecretEncryptor secretEncryptor, ISecretOwner owner);
    Task<IPopupResult> ShowCloneRequestPopup(IRequestTemplateCollectionService requestTemplateCollectionService, IMessageService messageService, FileModel file, RequestTemplateCollection collection, RequestTemplate request);
}
