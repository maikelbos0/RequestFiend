using Microsoft.Maui.Controls;
using System;

namespace RequestFiend.UI;

public partial class AppShell : Shell {
    public AppShell() {
        InitializeComponent();
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
}
