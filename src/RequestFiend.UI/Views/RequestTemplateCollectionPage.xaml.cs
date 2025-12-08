using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class RequestTemplateCollectionPage : ContentPage<RequestTemplateCollectionModel> {
    public RequestTemplateCollectionPage(RequestTemplateCollectionModel model) {
        Model = model;
        InitializeComponent();
    }
}
