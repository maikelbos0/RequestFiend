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
    private CancellationTokenSource? executingCts;

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
        using var cts = new CancellationTokenSource();

        if (Interlocked.CompareExchange(ref executingCts, cts, null) != null) {
            cts.Dispose();
            throw new InvalidOperationException("The request is already executing.");
        }

        executingCts = cts;

        PageTitleBase = $"{file.Name} - {request.Name} - Executing request...";
        ShellItemTitleBase = "Executing request...";
        IsExecuting = true;

        Context = await requestHandler.Execute(request, collection, cts.Token);

        PageTitleBase = $"{file.Name} - {request.Name} - Exchange";
        ShellItemTitleBase = "Exchange";
        IsExecuting = false;
        
        executingCts = null;
    }

    [RelayCommand]
    public void CancelExecution() {
        executingCts?.Cancel();
    }
}
