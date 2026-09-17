using CommunityToolkit.Maui.Views;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Services;

namespace RequestFiend.UI.Views;

public partial class EnvironmentPopup : Popup {
    public EnvironmentPopup(IEnvironmentService environmentService, IPopupService popupService, ISecretEncryptor secretEncryptor, FileModel file, Environment environment) {
        BindingContext = new EnvironmentModel(CloseAsync, environmentService, popupService, secretEncryptor, file, environment);
        InitializeComponent();
    }

    public async void OnCancelClicked(object sender, System.EventArgs e) {
        await CloseAsync();
    }
}