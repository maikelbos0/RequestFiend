using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestLogModel : PageBoundModelBase {
    private readonly IMessageService messageService;
    private readonly IPopupService popupService;
    private readonly int updateDelayInMilliseconds;
    private readonly ConcurrentQueue<string> logEventQueue = [];
    private int needsUpdating = 0;

    [ObservableProperty] public partial string LogEvents { get; set; } = "";

    public RequestLogModel(IMessageService messageService, IPopupService popupService, int updateDelayInMilliseconds) : base("Request log", "Request log") {
        this.messageService = messageService;
        this.popupService = popupService;
        this.updateDelayInMilliseconds = updateDelayInMilliseconds;
    }

    public void Add(string logEvent) {
        logEventQueue.Enqueue(logEvent);
        Interlocked.Exchange(ref needsUpdating, 1);
    }

    public async Task StartUpdating(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            if (Interlocked.CompareExchange(ref needsUpdating, 0, 1) == 1) {
                LogEvents = string.Join("", logEventQueue);
            }

#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods
            await Task.Delay(updateDelayInMilliseconds);
#pragma warning restore CA2016 // Forward the 'CancellationToken' parameter to methods
        }
    }

    [RelayCommand]
    public void Clear() {
        logEventQueue.Clear();
        Interlocked.Exchange(ref needsUpdating, 1);
    }

    [RelayCommand]
    public async Task Save() {
        var saveResult = await popupService.ShowSaveDialog(".log", new MemoryStream(Encoding.UTF8.GetBytes(LogEvents)));

        if (saveResult.IsSuccessful) {
            messageService.Send(new SuccessMessage("Response log has been saved"));
        }
        else if (saveResult.Exception != null && saveResult.Exception is not OperationCanceledException) {
            await popupService.ShowErrorPopup($"Failed to save response log: {saveResult.Exception.Message}");
        }
    }
}
