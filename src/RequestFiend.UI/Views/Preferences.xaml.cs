using Microsoft.Maui.Controls;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class Preferences : ContentPage {
    public Preferences(PreferencesModel model) : base(model) {
        InitializeComponent();

        model.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(PreferencesModel.ShellItemTitle) && Parent is BaseShellItem shellItem) {
                shellItem.Title = model.ShellItemTitle;
            }
        };
    }
}
