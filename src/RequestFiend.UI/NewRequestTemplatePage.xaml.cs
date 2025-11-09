using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class NewRequestTemplatePage : ContentPage<NewRequestTemplateModel> {
    private readonly RequestTemplateCollection collection;
    private readonly ShellItem parentItem;

    public NewRequestTemplatePage(ShellItem parentItem, RequestTemplateCollection collection) {
        this.collection = collection;
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

        var item = new Tab() {
            Icon = "paper_plane_solid_full.png",
            Title = request.Name,
            Items = {
                new RequestTemplatePage(collection, request)
            },
            Route = $"RequestTemplate_{Guid.NewGuid()}"
        };
        parentItem.Items.Add(item);

        await Shell.Current.GoToAsync($"//{parentItem.Route}/{item.Route}");

        Model.Reset();
    }
}