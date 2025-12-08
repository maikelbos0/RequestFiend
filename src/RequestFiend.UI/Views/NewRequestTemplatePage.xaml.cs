using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class NewRequestTemplatePage : ContentPage<NewRequestTemplateModel> {
    public NewRequestTemplatePage(NewRequestTemplateModel model) {
        Model = model;
        InitializeComponent();
    }
}
