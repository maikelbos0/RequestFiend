using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.UI.Messages;

namespace RequestFiend.UI;

public partial class RequestTemplatePage : RequestTemplateCollectionPageBase<RequestTemplateModel> {
    private readonly RequestTemplate request;

    public RequestTemplatePage(string filePath, RequestTemplateCollection collection, RequestTemplate request) : base(filePath, collection) {
        this.request = request;
        Model = new(request);
        InitializeComponent();
    }

    private async void OnUpdateRequestClicked(object sender, System.EventArgs e) {
        if (!Model.TryUpdateRequestTemplate(request)) {
            return;
        }

        await SaveCollection();
        WeakReferenceMessenger.Default.Send(new RequestTemplateUpdatedMessage(request), request.Id);

        await SuccessMessage.Show("Changes have been saved");
    }
}
