using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class RequestPage : ContentPage {
    public RequestPage(RequestModel model) : base(model) {
        InitializeComponent();
    }
}
