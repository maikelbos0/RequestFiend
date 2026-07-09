using CommunityToolkit.Maui.Views;
using RequestFiend.Models;
using System;

namespace RequestFiend.UI.Views;

public partial class EnvironmentPopup : Popup {
    public EnvironmentPopup(EnvironmentModel model) {
        BindingContext = model;
        InitializeComponent();
    }

    public async void OnCloseClicked(object sender, EventArgs e) {
        await CloseAsync();
    }
}