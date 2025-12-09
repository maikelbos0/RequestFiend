using RequestFiend.Core;
using RequestFiend.Models.Messages;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public class RequestTemplateCollectionService : IRequestTemplateCollectionService {
    private readonly IMessageService messageService;
    private readonly IFileSystem fileSystem;
    private readonly IRecentCollectionService recentCollectionService;

    public RequestTemplateCollectionService(IMessageService messageService, IFileSystem fileSystem, IRecentCollectionService recentCollectionService) {
        this.messageService = messageService;
        this.fileSystem = fileSystem;
        this.recentCollectionService = recentCollectionService;
    }

    public async Task Save(string filePath, RequestTemplateCollection collection) {
        await fileSystem.File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));

        messageService.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection), filePath);
        recentCollectionService.Push(filePath);
    }
}
