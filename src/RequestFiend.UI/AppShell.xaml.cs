using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.Models.Messages;
using RequestFiend.UI.Views;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class AppShell : Shell, IRecipient<SuccessMessage>, IRecipient<ErrorMessage> {
    private CancellationTokenSource? messageCancellationTokenSource;

    public AppShell() {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args) {
        TitleLabel.Text = CurrentItem.Title;
        CloseButton.IsVisible = CurrentItem.StyleId != null;
    }

    private async void OnCloseClicked(object sender, EventArgs e) {
        var currentItem = CurrentItem;
        await GoToAsync("//MainPage");
        Items.Remove(currentItem);
    }

    public async void Receive(SuccessMessage message) {
        this.messageCancellationTokenSource?.Cancel();

        var messageCancellationTokenSource = this.messageCancellationTokenSource = new();

        SuccessLabel.Text = message.Text;
        SuccessLabel.Opacity = 1;
        SuccessLabel.IsVisible = true;

        await Task.Delay(1000);

        for (var i = 0; i < 25; i++) {
            if (messageCancellationTokenSource.Token.IsCancellationRequested) {
                return;
            }

            SuccessLabel.Opacity = (25 - i) / 25.0;
            await Task.Delay(40);
        }

        SuccessLabel.IsVisible = false;
    }

    public async void Receive(ErrorMessage message) {
        await this.ShowPopupAsync(new ErrorPopup(message.Text));
    }
}
