using CommunityToolkit.Maui.Views;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Services;

namespace RequestFiend.UI.Views;

public partial class EnvironmentPopup : Popup {
    public EnvironmentPopup(IEnvironmentService environmentService, FileModel file, Environment environment) {
        BindingContext = new EnvironmentModel(CloseAsync, environmentService, file, environment);
        InitializeComponent();
    }
}