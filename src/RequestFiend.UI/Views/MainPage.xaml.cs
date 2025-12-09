using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class MainPage : ContentPage<MainPageModel> {
    public MainPage(MainPageModel model) {
        Model = model;
        InitializeComponent();
    }
}
