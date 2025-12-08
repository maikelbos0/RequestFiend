using RequestFiend.Core;
using RequestFiend.Models.Messages;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public class RequestTemplateCollectionService : IRequestTemplateCollectionService {
    private readonly IMessageService messageService;
    private readonly IFileSystem fileSystem;

    public RequestTemplateCollectionService(IMessageService messageService, IFileSystem fileSystem) {
        this.messageService = messageService;
        this.fileSystem = fileSystem;
    }

    public async Task Save(string filePath, RequestTemplateCollection collection) {
        await fileSystem.File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));

        messageService.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection));
        messageService.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection), filePath);
    }
}
