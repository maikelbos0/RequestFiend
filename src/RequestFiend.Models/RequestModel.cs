using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestModel : BoundModelBase, IRequestExchangeListener, IDisposable {
    private readonly IMessageService messageService;
    private readonly IRequestHandler requestHandler;
    private readonly RequestTemplateCollectionFileModel file;
    private readonly RequestTemplateCollection collection;
    private readonly RequestTemplate request;
    private CancellationTokenSource? executingCancellationTokenSource;

    public string Id { get; } = Guid.NewGuid().ToString();
    public bool IsExecuting { get => field; set => SetProperty(ref field, value); }
    public HttpRequestModel? Request { get => field; set => SetProperty(ref field, value); }
    public HttpResponseModel? Response { get => field; set => SetProperty(ref field, value); }
    public ExceptionModel? Exception { get => field; set => SetProperty(ref field, value); }

    public RequestModel(
        IMessageService messageService,
        IRequestHandler requestHandler,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection,
        RequestTemplate request
    ) : base($"{file.Name} - {request.Name} - Exchange", $"{request.Name} - Exchange") {
        this.messageService = messageService;
        this.requestHandler = requestHandler;
        this.file = file;
        this.collection = collection;
        this.request = request;

        ConfigureState([]);
    }

    [RelayCommand]
    public async Task Execute() {
        using var cancellationTokenSource = new CancellationTokenSource();

        if (Interlocked.CompareExchange(ref executingCancellationTokenSource, cancellationTokenSource, null) != null) {
            cancellationTokenSource.Dispose();
            throw new InvalidOperationException("The request is already executing.");
        }

        executingCancellationTokenSource = cancellationTokenSource;
        PageTitleBase = $"{file.Name} - {request.Name} - Executing request...";
        ShellItemTitleBase = $"{request.Name} - Executing request...";
        IsExecuting = true;
        Request = null;
        Response = null;
        Exception = null;

        await requestHandler.Execute(request, collection, this, cancellationTokenSource.Token);

        PageTitleBase = $"{file.Name} - {request.Name} - Exchange";
        ShellItemTitleBase = $"{request.Name} - Exchange";
        IsExecuting = false;
        executingCancellationTokenSource = null;
    }

    [RelayCommand]
    public void CancelExecution() {
        executingCancellationTokenSource?.Cancel();
    }

    [RelayCommand]
    public void Close() {
        executingCancellationTokenSource?.Cancel();
        messageService.Send(new CloseRequestMessage(), Id);
    }

    public Task OnRequestCreated(HttpRequestMessage request) {
        Request = HttpRequestModel.Create(request);
        return Task.CompletedTask;
    }

    public async Task OnResponseReceived(HttpResponseMessage response)
        => Response = await HttpResponseModel.Create(response);

    public Task OnExceptionCaught(Exception exception) { 
        Exception = new(exception);
        return Task.CompletedTask;
    }

    public void Dispose() {
        executingCancellationTokenSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}
