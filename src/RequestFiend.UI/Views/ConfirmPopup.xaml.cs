using CommunityToolkit.Maui.Views;
using System;

namespace RequestFiend.UI.Views;

public partial class ConfirmPopup : Popup<bool> {
    public ConfirmPopup(string message) {
        InitializeComponent();
        MessageLabel.Text = message;
    }

    public async void OnOkClicked(object sender, EventArgs e) {
        await CloseAsync(true);
    }

    public async void OnCancelClicked(object sender, EventArgs e) {
        await CloseAsync(false);
    }
}
