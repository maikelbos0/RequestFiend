using Microsoft.Maui.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.UI.Views;

public partial class Message : Label {
    private CancellationTokenSource? messageCancellationTokenSource;

    public Message() {
        InitializeComponent();
    }

    public async Task Show(string text) {
        this.messageCancellationTokenSource?.Cancel();

        var messageCancellationTokenSource = this.messageCancellationTokenSource = new();

        Text = text;
        Opacity = 1;
        IsVisible = true;

        await Task.Delay(1000);

        for (var i = 0; i < 50; i++) {
            if (messageCancellationTokenSource.Token.IsCancellationRequested) {
                return;
            }

            Opacity = (50 - i) / 50.0;
            await Task.Delay(20);
        }

        IsVisible = false;
    }
}
