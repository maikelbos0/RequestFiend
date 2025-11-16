using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.UI.Messages;
using System;

namespace RequestFiend.UI;

// TODO maybe use idiom to determine button orientation
// https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device/information?view=net-maui-10.0&tabs=android
public partial class RequestTemplatePage : RequestTemplateCollectionPageBase<RequestTemplateModel> {
    private readonly RequestTemplate request;

    public RequestTemplatePage(string filePath, RequestTemplateCollection collection, RequestTemplate request) : base(filePath, collection) {
        this.request = request;
        Model = new(request);
        InitializeComponent();
    }

    private async void OnUpdateRequestClicked(object sender, EventArgs e) {
        if (!Model.TryUpdateRequestTemplate(request)) {
            return;
        }

        await SaveCollection();
        WeakReferenceMessenger.Default.Send(new RequestTemplateUpdatedMessage(request), request.Id);

        await SuccessMessage.Show("Changes have been saved");
    }

    private async void OnDeleteRequestClicked(object sender, EventArgs e) {
        var popupResult = await this.ShowPopupAsync<bool>(new Views.ConfirmPopup("Are you sure you want to delete this request?"));

        if (!popupResult.WasDismissedByTappingOutsideOfPopup && popupResult.Result) {
            collection.Requests.Remove(request);
            await SaveCollection();
            WeakReferenceMessenger.Default.Send(new RequestTemplateDeletedMessage(), request.Id);
        }
    }
}
