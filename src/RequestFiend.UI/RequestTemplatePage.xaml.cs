using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.UI.Messages;
using System;

namespace RequestFiend.UI;

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

    public void OnRemovetHeaderClicked(object sender, EventArgs e) {
        var button = (Button)sender;

        Model.Headers.Remove((HeaderTemplateModel)button.BindingContext);
    }

    public void OnAddHeaderClicked(object sender, EventArgs e) => Model.Headers.Add(new());

    private async void OnDeleteRequestClicked(object sender, EventArgs e) {
        var popupResult = await this.ShowPopupAsync<bool>(new Views.ConfirmPopup("Are you sure you want to delete this request?"));

        if (!popupResult.WasDismissedByTappingOutsideOfPopup && popupResult.Result) {
            collection.Requests.Remove(request);
            await SaveCollection();
            WeakReferenceMessenger.Default.Send(new RequestTemplateDeletedMessage(), request.Id);
        }
    }
}
