using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Messages;
using System;

namespace RequestFiend.UI.Views;

public partial class RequestTemplateCollectionPage : RequestTemplateCollectionPageBase<RequestTemplateCollectionModel> {
    public RequestTemplateCollectionPage(string filePath, RequestTemplateCollection collection) : base(filePath, collection) {
        if (collection.DefaultHeaders.Count == 0) {
            collection.DefaultHeaders.Add(new() { Name = "Accept", Value = "application/json" });
            collection.DefaultHeaders.Add(new() { Name = "X-api-key", Value = Guid.NewGuid().ToString() });
        }
        Model = new(filePath, collection);
        InitializeComponent();
    }

    private async void OnUpdateCollectionClicked(object sender, EventArgs e) {
        if (!Model.TryUpdateRequestTemplateCollection(collection)) {
            return;
        }

        await SaveCollection();

        WeakReferenceMessenger.Default.Send(new SuccessMessage("Changes have been saved"));
    }
}
