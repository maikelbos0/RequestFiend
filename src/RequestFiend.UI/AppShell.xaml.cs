using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.UI.Messages;
using System;

namespace RequestFiend.UI;

public partial class AppShell : Shell, IRecipient<RequestTemplateCollectionUpdatedMessage> {
    public AppShell() {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register(this);
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

    public void Receive(RequestTemplateCollectionUpdatedMessage message) {
        if (CurrentItem.StyleId == message.FilePath) {
            TitleLabel.Text = message.Collection.Name;
        }
    }
}
