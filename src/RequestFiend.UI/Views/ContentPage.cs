using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class ContentPage : Microsoft.Maui.Controls.ContentPage {
    public ContentPage(BoundModelBase model) {
        BindingContext = model;
    }

    protected override void OnSizeAllocated(double width, double height) {
        base.OnSizeAllocated(width, height);

        ((BoundModelBase)BindingContext).PageWidth = Width;
    }
}
