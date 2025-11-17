using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models;
using System;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : RequestTemplateCollectionPageBase<RequestTemplateCollectionModel> {
    public RequestTemplateCollectionPage(string filePath, RequestTemplateCollection collection) : base(filePath, collection) {
        if (collection.DefaultHeaders.Count == 0) {
            collection.DefaultHeaders.Add(new() { Name = "Accept", Value = "application/json" });
            collection.DefaultHeaders.Add(new() { Name = "X-api-key", Value = Guid.NewGuid().ToString() });
        }
        Model = new(collection);
        InitializeComponent();
    }

    public void OnRemoveDefaultHeaderClicked(object sender, EventArgs e) {
        var button = (Button)sender;
        
        Model.DefaultHeaders.Remove((HeaderTemplateModel)button.BindingContext);
    }

    private async void OnUpdateCollectionClicked(object sender, EventArgs e) {
        if (!Model.TryUpdateRequestTemplateCollection(collection)) {
            return;
        }

        await SaveCollection();

        await SuccessMessage.Show("Changes have been saved");
    }

    public Task ShowMessage(string text) => SuccessMessage.Show(text);
}
