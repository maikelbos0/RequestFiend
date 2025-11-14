using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Messages;
using RequestFiend.UI.Models;
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

        var item = new Tab() {
            Icon = "paper_plane_solid_full.png",
            Title = request.Name,
            Items = {
                new RequestTemplatePage(filePath, collection, request)
            },
            Route = $"RequestTemplate_{Guid.NewGuid()}"
        };
        WeakReferenceMessenger.Default.Register<Tab, RequestTemplateUpdatedMessage, Guid>(item, request.Id, (tab, message) => tab.Title = request.Name);
        collectionItem.Items.Add(item);

        await Shell.Current.GoToAsync($"//{collectionItem.Route}/{item.Route}");

        Model.Reset();
    }
}