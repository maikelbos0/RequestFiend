using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class ExchangePage : ContentPage {
    public ExchangePage(ExchangeModel model) : base(model) {
        InitializeComponent();
        _ = model.Execute();
    }
}
