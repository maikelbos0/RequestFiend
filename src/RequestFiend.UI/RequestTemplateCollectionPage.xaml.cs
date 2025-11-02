using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : ContentPage {
    public static async Task Open(RequestTemplateCollection collection, string filePath) {
        var page = new RequestTemplateCollectionPage(collection, filePath);
        var shellContent = new ShellContent() {
            Title = collection.Name,
            Content = page,
            Route = $"RequestTemplateCollection_{Guid.NewGuid()}"
        };

        Shell.Current.Items.Add(shellContent);
        await Shell.Current.GoToAsync($"//{shellContent.Route}");
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
