using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestModel : BoundModelBase {
    private readonly IRequestHandler requestHandler;
    private readonly RequestTemplateCollection collection;
    private readonly RequestTemplate request;

    public Guid Id { get; } = Guid.NewGuid();
    public RequestContext? Context { get => field; set => SetProperty(ref field, value); }

    public RequestModel(
        IRequestHandler requestHandler,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection,
        RequestTemplate request
    ) : base($"{file.Name} - Executing {request.Name}", $"Executing {request.Name}") {
        this.requestHandler = requestHandler;
        this.collection = collection;
        this.request = request;
    }

    [RelayCommand]
    public async Task Execute(CancellationToken cancellationToken)
        => Context = await requestHandler.Execute(request, collection, cancellationToken);
}
