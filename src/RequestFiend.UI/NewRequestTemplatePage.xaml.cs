using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class NewRequestTemplatePage : RequestTemplateCollectionPageBase<NewRequestTemplateModel> {
    private readonly ShellItem parentItem;

    public NewRequestTemplatePage(string filePath, RequestTemplateCollection collection, ShellItem parentItem) : base(filePath, collection) {
        this.parentItem = parentItem;
        Model = new(collection);
        InitializeComponent();
        Title = collection.Name;
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
        parentItem.Items.Add(item);

        await Shell.Current.GoToAsync($"//{parentItem.Route}/{item.Route}");

        Model.Reset();
    }
}