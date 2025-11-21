using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models;
using System;
using System.Threading.Tasks;

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

        await SuccessMessage.Show("Changes have been saved");
    }

    public void OnRemoveDefaultHeaderClicked(object sender, EventArgs e) {
        var button = (Button)sender;

        Model.DefaultHeaders.Remove((NameValuePairModel)button.BindingContext);
    }

    public void OnAddDefaultHeaderClicked(object sender, EventArgs e) => Model.DefaultHeaders.Add(new());

    public void OnRemoveVariableClicked(object sender, EventArgs e) {
        var button = (Button)sender;

        Model.Variables.Remove((NameValuePairModel)button.BindingContext);
    }

    public void OnAddVariableClicked(object sender, EventArgs e) => Model.Variables.Add(new());

    public Task ShowMessage(string text) => SuccessMessage.Show(text);
}
