using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestModel : BoundModelBase {
    private readonly IRequestHandler requestHandler;
    private readonly RequestTemplateCollectionFileModel file;
    private readonly RequestTemplateCollection collection;
    private readonly RequestTemplate request;

    public string Id { get; } = Guid.NewGuid().ToString();
    public bool IsExecuting { get => field; set => SetProperty(ref field, value); }
    public RequestContext? Context { get => field; set => SetProperty(ref field, value); }

    public RequestModel(
        IRequestHandler requestHandler,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection,
        RequestTemplate request
    ) : base($"{file.Name} - {request.Name} - Exchange", "Exchange") {
        this.requestHandler = requestHandler;
        this.file = file;
        this.collection = collection;
        this.request = request;

        ConfigureState([]);
    }

    [RelayCommand]
    public async Task Execute() {
        PageTitleBase = $"{file.Name} - {request.Name} - Executing request...";
        ShellItemTitleBase = "Executing request...";
        IsExecuting = true;
        Context = await requestHandler.Execute(request, collection, CancellationToken.None);
        PageTitleBase = $"{file.Name} - {request.Name} - Exchange";
        ShellItemTitleBase = "Exchange";
        IsExecuting = false;
    }
}
