using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class CloneRequestTemplateModel : BoundModelBase {
    private readonly Func<CancellationToken, Task> closeMethod;
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IMessageService messageService;
    private readonly FileModel file;
    private readonly RequestTemplateCollection collection;
    private readonly RequestTemplate request;

    public ValidatableProperty<string> Name { get; }

    public CloneRequestTemplateModel(
        Func<CancellationToken, Task> closeMethod,
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IMessageService messageService,
        FileModel file,
        RequestTemplateCollection collection,
        RequestTemplate request
    ) {
        this.closeMethod = closeMethod;
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.messageService = messageService;

        this.file = file;
        this.collection = collection;
        this.request = request;

        Name = new(() => request.Name, value => request.Name = value, Validator.Required);

        ConfigureState([Name]);
    }

    [RelayCommand]
    public async Task Clone(CancellationToken cancellationToken) {
        if (HasError) {
            return;
        }

        Set();

        collection.Requests.Add(request);

        await requestTemplateCollectionService.Save(file.FilePath, collection);
        messageService.Send(new RequestTemplateCreatedMessage(file.FilePath, collection, request));
        messageService.Send(new RequestTemplateAddedToCollectionMessage(request), file);
        messageService.Send(new SuccessMessage("Request has been cloned"));

        await closeMethod(cancellationToken);
    }
}
