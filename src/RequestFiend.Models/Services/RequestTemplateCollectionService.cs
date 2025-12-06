using RequestFiend.Core;
using RequestFiend.Models.Messages;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public class RequestTemplateCollectionService : IRequestTemplateCollectionService {
    private readonly IMessageService messageService;
    private readonly IFileSystem fileSystem;
    private readonly string filePath;

    public RequestTemplateCollection Collection { get; }
    public string Title { get; }

    public RequestTemplateCollectionService(IMessageService messageService, IFileSystem fileSystem, ITransientDataProvider<(string, RequestTemplateCollection)> transientDataProvider) {
        this.messageService = messageService;
        this.fileSystem = fileSystem;

        (filePath, Collection) = transientDataProvider.GetData();
        Title = fileSystem.Path.GetFileNameWithoutExtension(filePath);
    }

    public async Task Save() {
        await fileSystem.File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(Collection));

        messageService.Send(new RequestTemplateCollectionUpdatedMessage(filePath, Collection));
        messageService.Send(new RequestTemplateCollectionUpdatedMessage(filePath, Collection), filePath);
    }
}
