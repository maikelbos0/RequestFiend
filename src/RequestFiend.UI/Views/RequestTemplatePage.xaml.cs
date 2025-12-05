using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Messages;
using System;

namespace RequestFiend.UI.Views;

public partial class RequestTemplatePage : RequestTemplateCollectionPageBase<RequestTemplateModel> {
    private readonly RequestTemplate request;

    public RequestTemplatePage(string filePath, RequestTemplateCollection collection, RequestTemplate request, RequestTemplateModel model) : base(filePath, collection) {
        this.request = request;
        Model = model;
        InitializeComponent();
    }

    private async void OnUpdateRequestClicked(object sender, EventArgs e) {
        if (!Model.TryUpdateRequestTemplate(request)) {
            return;
        }

        await SaveCollection();
        WeakReferenceMessenger.Default.Send(new RequestTemplateUpdatedMessage(request), request.Id);
        WeakReferenceMessenger.Default.Send(new SuccessMessage("Changes have been saved"));
    }

    private async void OnValidateJsonClicked(object sender, EventArgs e) {
        if (Model.ValidateJson(out var exception)) {
            WeakReferenceMessenger.Default.Send(new SuccessMessage("JSON content has been validated"));
        }
        else {
            WeakReferenceMessenger.Default.Send(new ErrorMessage($"Failed to validate JSON content: {exception.Message}"));
        }
    }

    private async void OnFormatJsonClicked(object sender, EventArgs e) {
        if (Model.FormatJson(out var exception)) {
            WeakReferenceMessenger.Default.Send(new SuccessMessage("JSON content has been formatted"));
        }
        else {
            WeakReferenceMessenger.Default.Send(new ErrorMessage($"Failed to format JSON content: {exception.Message}"));
        }
    }
}
