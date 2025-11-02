using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : ContentPage {
    public static async Task Open(RequestTemplateCollection collection, string filePath) {
        var item = new FlyoutItem() {
            Title = collection.Name,
            Icon = "folder_open_solid_full.png",
            Route = $"RequestTemplateCollection_{Guid.NewGuid()}"
        };

        item.Items.Add(new Tab() {
            Title = "Collection settings",
            Icon =  "bars_solid_full.png",
            Items = {
                new RequestTemplateCollectionPage(collection, filePath)
            }
        });

        foreach (var request in collection.Requests) {
            item.Items.Add(new Tab() {
                Icon = "paper_plane_solid_full.png",
                Title = request.Name,
                Items = {
                    // TODO create request template page
                    new RequestTemplateCollectionPage(collection, filePath)
                }
            });
        }

        Shell.Current.Items.Add(item);
        await Shell.Current.GoToAsync($"//{item.Route}");
    }

    public RequestTemplateCollectionModel Model {
        get => BindingContext as RequestTemplateCollectionModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    public RequestTemplateCollectionPage(RequestTemplateCollection collection, string filePath) {
        InitializeComponent();
        Model = new() {
            Collection = collection,
            FilePath = filePath
        };
        Title = collection.Name;
    }
}
