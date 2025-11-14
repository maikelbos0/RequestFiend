using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models;
using System;

namespace RequestFiend.UI;

public partial class NewRequestTemplatePage : RequestTemplateCollectionPageBase<NewRequestTemplateModel> {
    private readonly ShellItem collectionItem;

    public NewRequestTemplatePage(string filePath, RequestTemplateCollection collection, ShellItem collectionItem) : base(filePath, collection) {
        this.collectionItem = collectionItem;
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