using CommunityToolkit.Maui.Views;
using System;

namespace RequestFiend.UI.Views;

public partial class ErrorPopup : Popup {
    public ErrorPopup(string message) {
        InitializeComponent();
        MessageLabel.Text = message;
    }

    public async void OnDismissClicked(object sender, EventArgs e) {
        await CloseAsync();
    }
}
