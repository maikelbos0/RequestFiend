using RequestFiend.Core;
using RequestFiend.UI.Models;

namespace RequestFiend.UI;

public partial class RequestTemplatePage : ContentPage<RequestTemplateModel> {
    private readonly RequestTemplateCollection collection;
    private readonly RequestTemplate request;

    public RequestTemplatePage(RequestTemplateCollection collection, RequestTemplate request) {
        this.collection = collection;
        this.request = request;
        Model = new();
        InitializeComponent();
        Title = collection.Name;
    }
}
