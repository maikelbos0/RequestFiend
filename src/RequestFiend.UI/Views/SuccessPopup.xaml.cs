using CommunityToolkit.Maui.Views;
using System;

namespace RequestFiend.UI.Views;

public partial class SuccessPopup : Popup {
    public SuccessPopup(string message) {
        InitializeComponent();
        MessageLabel.Text = message;
    }

    public async void OnDismissClicked(object sender, EventArgs e) {
        await CloseAsync();
    }
}
