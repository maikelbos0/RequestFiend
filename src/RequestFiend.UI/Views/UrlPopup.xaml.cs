using CommunityToolkit.Maui.Views;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Services;

namespace RequestFiend.UI.Views;

public partial class UrlPopup : Popup<string?> {
    public UrlPopup(IEnvironmentService environmentService, RequestTemplateCollection collection, string url) {
        BindingContext = new UrlModel(CloseAsync, environmentService, collection, url);
        InitializeComponent();
    }
}