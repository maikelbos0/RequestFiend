using Microsoft.Maui.Controls;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class MainPage : ContentPage {
    public MainPage(MainPageModel model) : base(model) {
        InitializeComponent();

        model.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(MainPageModel.ShellItemTitle) && Parent is BaseShellItem shellItem) {
                shellItem.Title = model.ShellItemTitle;
            }
        };
    }
}
