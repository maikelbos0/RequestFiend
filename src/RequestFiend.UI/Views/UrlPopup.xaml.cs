using CommunityToolkit.Maui.Views;
using RequestFiend.Core;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class UrlPopup : Popup<string?> {
    public UrlPopup(RequestTemplateCollection collection, string url) {
        BindingContext = new UrlModel(CloseAsync, collection, url);
        InitializeComponent();
    }
}