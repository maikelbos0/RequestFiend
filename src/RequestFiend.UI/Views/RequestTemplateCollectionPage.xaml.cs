using RequestFiend.Core;
using RequestFiend.Models;
using System;

namespace RequestFiend.UI.Views;

public partial class RequestTemplateCollectionPage : RequestTemplateCollectionPageBase<RequestTemplateCollectionModel> {
    public RequestTemplateCollectionPage(string filePath, RequestTemplateCollection collection) : base(filePath, collection) {
        if (collection.DefaultHeaders.Count == 0) {
            collection.DefaultHeaders.Add(new() { Name = "Accept", Value = "application/json" });
            collection.DefaultHeaders.Add(new() { Name = "X-api-key", Value = Guid.NewGuid().ToString() });
        }
        Model = new(collection);
        InitializeComponent();
    }

    private async void OnUpdateCollectionClicked(object sender, EventArgs e) {
        if (!Model.TryUpdateRequestTemplateCollection(collection)) {
            return;
        }

        await SaveCollection();

        await ShowSuccess("Changes have been saved");
    }
}
