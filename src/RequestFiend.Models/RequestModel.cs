using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MimeMapping;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestModel : PageBoundModelBase, IRequestExchangeListener, IDisposable {
    private readonly IMessageService messageService;
    private readonly IRequestHandler requestHandler;
    private readonly IPopupService popupService;
    private readonly IPreferencesService preferencesService;
    private readonly IUserInterface userInterface;
    private readonly RequestTemplateCollectionFileModel file;
    private readonly RequestTemplateCollection collection;
    private readonly RequestTemplate request;
    private CancellationTokenSource? executingCancellationTokenSource;

    public string Id { get; } = Guid.NewGuid().ToString();
    [ObservableProperty] public partial bool IsExecuting { get; set; }
    [ObservableProperty] public partial HttpRequestModel? Request { get; set; }
    [ObservableProperty] public partial HttpResponseModel? Response { get; set; }
    [ObservableProperty] public partial ExceptionModel? Exception { get; set; }

    public RequestModel(
        IMessageService messageService,
        IRequestHandler requestHandler,
        IPopupService popupService,
        IPreferencesService preferencesService,
        IUserInterface userInterface,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection,
        RequestTemplate request
    ) : base($"{file.Name} - {request.Name} - Exchange", $"{request.Name} - Exchange") {
        this.messageService = messageService;
        this.requestHandler = requestHandler;
        this.popupService = popupService;
        this.preferencesService = preferencesService;
        this.userInterface = userInterface;

        this.file = file;
        this.collection = collection;
        this.request = request;

        ConfigureState([]);
    }

    [RelayCommand]
    public async Task Execute() {
        var requestTimeoutInSeconds = preferencesService.GetRequestTimeoutInSeconds();
        using var cancellationTokenSource = requestTimeoutInSeconds.HasValue
            ? new CancellationTokenSource(TimeSpan.FromSeconds(requestTimeoutInSeconds.Value))
            : new CancellationTokenSource();

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

        var scriptEvaluationMode = preferencesService.GetScriptEvaluationMode();
        var options = new RequestExchangeOptions(
            preferencesService.GetScriptEvaluationMode() switch {
                ScriptEvaluationMode.Disabled => false,
                ScriptEvaluationMode.Enabled => true,
                ScriptEvaluationMode.CollectionScoped => preferencesService.GetCollectionAllowScriptEvaluation(file.FilePath),
                _ => throw new NotImplementedException($"Received unknown script evaluation mode '{scriptEvaluationMode}'.")
            }
        );

        await Task.Run(() => requestHandler.Execute(request, collection, options, this, cancellationTokenSource.Token));

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
    public async Task SaveResponseContent() {
        if (Response?.Content?.BinaryContent == null) {
            return;
        }

        var saveResult = await popupService.ShowSaveDialog(GetExtension(), new MemoryStream(Response.Content.BinaryContent));

        if (saveResult.IsSuccessful) {
            messageService.Send(new SuccessMessage("Response content has been saved"));
        }
        else if (saveResult.Exception != null && saveResult.Exception is not OperationCanceledException) {
            await popupService.ShowErrorPopup($"Failed to save response content: {saveResult.Exception.Message}");
        }

        string GetExtension() {
            if (Response.Content.MediaType != null) {
                var extensions = MimeUtility.GetExtensions(Response.Content.MediaType);

                if (extensions != null) {
                    return $".{extensions[0]}";
                }
            }

            if (Response.Content.Type == HttpContentType.Text) {
                return ".txt";
            }

            return ".bin";
        }
    }

    [RelayCommand]
    public void Close() {
        executingCancellationTokenSource?.Cancel();
        messageService.Send(new CloseRequestMessage(), Id);
    }

    public Task OnRequestCreated(HttpRequestMessage request) {
        var model = HttpRequestModel.Create(request);
        userInterface.BeginInvokeOnMainThread(() => Request = model);
        return Task.CompletedTask;
    }

    public async Task OnResponseReceived(HttpResponseMessage response) {
        var model = await HttpResponseModel.Create(response);
        userInterface.BeginInvokeOnMainThread(() => Response = model);
    }

    public Task OnExceptionCaught(Exception exception) {
        var model = ExceptionModel.Create(exception);
        userInterface.BeginInvokeOnMainThread(() => Exception = model);
        return Task.CompletedTask;
    }

    public void Dispose() {
        executingCancellationTokenSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}
