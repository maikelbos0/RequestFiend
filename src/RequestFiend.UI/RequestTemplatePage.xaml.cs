using RequestFiend.Core;
using RequestFiend.UI.Models;

namespace RequestFiend.UI;

public partial class RequestTemplatePage : RequestTemplateCollectionPageBase<RequestTemplateModel> {
    private readonly RequestTemplate request;

    public RequestTemplatePage(string filePath, RequestTemplateCollection collection, RequestTemplate request) : base(filePath, collection) {
        this.request = request;
        Model = new();
        InitializeComponent();
        Title = collection.Name;
    }
}
