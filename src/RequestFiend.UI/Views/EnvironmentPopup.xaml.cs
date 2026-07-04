using CommunityToolkit.Maui.Views;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class EnvironmentPopup : Popup {
    public EnvironmentPopup(EnvironmentModel model) {
        BindingContext = model;
        InitializeComponent();
    }
}