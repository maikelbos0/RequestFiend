using RequestFiend.Core;
using RequestFiend.UI.Models;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : ContentPage<RequestTemplateCollectionModel> {
    private readonly RequestTemplateCollection collection;
    private readonly string filePath;

    public RequestTemplateCollectionPage(RequestTemplateCollection collection, string filePath) {
        this.collection = collection;
        this.filePath = filePath;
        Model = new();
        InitializeComponent();
        Title = collection.Name;
    }
}
