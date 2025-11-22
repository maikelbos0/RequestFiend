using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models;
using System;

namespace RequestFiend.UI.Views;

public partial class NewRequestTemplatePage : RequestTemplateCollectionPageBase<NewRequestTemplateModel> {
    public NewRequestTemplatePage(string filePath, RequestTemplateCollection collection) : base(filePath, collection) {
        // TODO if collection default url changes, trigger reset of url maybe only if equal to default url?
        Model = new(collection);
        InitializeComponent();
    }

    private async void OnCreateRequestTemplateClicked(object sender, EventArgs e) {
        if (!Model.TryCreateRequestTemplate(out var request)) {
            return;
        }

        collection.Requests.Add(request);
        await SaveCollection();
        await Shell.Current.OpenRequest(filePath, collection, request);

        Model.Reset();
    }
}