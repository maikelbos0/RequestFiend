using RequestFiend.Models;
using System.Threading;

namespace RequestFiend.UI.Views;

public partial class ExchangeLogPage : ContentPage {
    private CancellationTokenSource? updatingCancellationTokenSource;

    public ExchangeLogPage(ExchangeLogModel model) : base(model) {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(Microsoft.Maui.Controls.NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);

        var cancellationTokenSource = new CancellationTokenSource();

        if (Interlocked.CompareExchange(ref updatingCancellationTokenSource, cancellationTokenSource, null) != null) {
            cancellationTokenSource.Dispose();
        }

        _ = ((ExchangeLogModel)BindingContext).StartUpdating(cancellationTokenSource.Token);
    }

    protected override void OnNavigatingFrom(Microsoft.Maui.Controls.NavigatingFromEventArgs args) {
        base.OnNavigatingFrom(args);

        var cancellationTokenSource = updatingCancellationTokenSource;

        if (cancellationTokenSource != null) {
            updatingCancellationTokenSource = null;
            cancellationTokenSource.Cancel();
        }
    }
}