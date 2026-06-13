using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestLogModel : PageBoundModelBase {
    private readonly ConcurrentQueue<string> logEventQueue = [];
    private readonly int updateDelayInMilliseconds;
    private int needsUpdating = 0;

    [ObservableProperty] public partial string LogEvents { get; private set; } = "";

    public RequestLogModel(int updateDelayInMilliseconds) : base("Request log", "Request log") {
        this.updateDelayInMilliseconds = updateDelayInMilliseconds;
    }

    public void Add(string logEvent) {
        logEventQueue.Enqueue(logEvent);
        Interlocked.Exchange(ref needsUpdating, 1);
    }

    [RelayCommand]
    public void Clear() {
        logEventQueue.Clear();
        Interlocked.Exchange(ref needsUpdating, 1);
    }

    public async Task StartUpdating(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            if (Interlocked.CompareExchange(ref needsUpdating, 0, 1) == 1) {
                LogEvents = string.Join("", logEventQueue);
            }

            await Task.Delay(updateDelayInMilliseconds);
        }
    }
}
