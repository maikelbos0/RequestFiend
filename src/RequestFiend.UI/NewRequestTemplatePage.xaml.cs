using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class NewRequestTemplatePage : ContentPage {
    private readonly RequestTemplateCollection collection;
    private readonly ShellItem parentItem;

    public NewRequestTemplateModel Model {
        get => BindingContext as NewRequestTemplateModel ?? throw new InvalidOperationException();
        set => BindingContext = value;
    }

    public NewRequestTemplatePage(ShellItem parentItem, RequestTemplateCollection collection) {
        this.collection = collection;
        this.parentItem = parentItem;
        Model = new() {
            Url = collection.DefaultUrl
        };
        InitializeComponent();
        Title = collection.Name;
    }

    private async void OnCreateRequestTemplateClicked(object sender, EventArgs e) {
        var request = new RequestTemplate() {
            Name = Model.Name,
            Method = Model.Method,
            Url = Model.Url
        };
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

        Model = new() {
            Url = collection.DefaultUrl
        };
    }
}