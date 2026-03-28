using CommunityToolkit.Maui.Views;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class UrlPopup : Popup<string?> {
    public UrlPopup(string url) {
        BindingContext = new UrlModel(CloseAsync, url);
        InitializeComponent();
    }
}