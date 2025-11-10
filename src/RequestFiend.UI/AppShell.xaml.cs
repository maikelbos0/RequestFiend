using Microsoft.Maui.Controls;
using System;

namespace RequestFiend.UI;

public partial class AppShell : Shell {
    public AppShell() {
        InitializeComponent();
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args) {
        SetTitle(CurrentItem.Title);
        CloseButton.IsVisible = CurrentItem.StyleId != null;
    }

    public void SetTitle(string title) {
        TitleLabel.Text = title;
    }

    private async void OnCloseClicked(object sender, EventArgs e) {
        var currentItem = CurrentItem;
        await GoToAsync("//MainPage");
        Items.Remove(currentItem);
    }
}
