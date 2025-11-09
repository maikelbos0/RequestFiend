using Microsoft.Maui.Controls;
using System;

namespace RequestFiend.UI;

public partial class AppShell : Shell {
    public AppShell() {
        InitializeComponent();
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args) {
        SetTitle(Current.CurrentItem.Title);
        CloseButton.IsVisible = Current.CurrentItem.StyleId != null;
    }

    public void SetTitle(string title) {
        TitleLabel.Text = title;
    }

    private async void OnCloseClicked(object sender, EventArgs e) {
        var currentItem = Current.CurrentItem;
        await Current.GoToAsync("//MainPage");
        Current.Items.Remove(currentItem);
    }
}
