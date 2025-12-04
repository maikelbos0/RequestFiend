using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public class RequestTemplateCollectionModelBase : BoundModelBase {
    private readonly IFileService fileService;
    protected readonly string filePath;
    protected readonly RequestTemplateCollection collection;

    public string Title { get; }

    public RequestTemplateCollectionModelBase(IFileService fileService, string filePath, RequestTemplateCollection collection) {
        this.fileService = fileService;
        this.filePath = filePath;
        this.collection = collection;
        Title = Path.GetFileNameWithoutExtension(filePath);
    }

    public async Task SaveCollection() {
        await fileService.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));

        WeakReferenceMessenger.Default.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection));
        WeakReferenceMessenger.Default.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection), filePath);
    }
}
