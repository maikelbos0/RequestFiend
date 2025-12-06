using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class RequestTemplatePage : ContentPage<RequestTemplateModel> {
    public RequestTemplatePage(RequestTemplateModel model) {
        Model = model;
        InitializeComponent();
    }
}
