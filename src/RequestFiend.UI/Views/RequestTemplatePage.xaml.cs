using RequestFiend.Core;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class RequestTemplatePage : RequestTemplateCollectionPageBase<RequestTemplateModel> {
    public RequestTemplatePage(string filePath, RequestTemplateCollection collection, RequestTemplateModel model) : base(filePath, collection) {
        Model = model;
        InitializeComponent();
    }
}
