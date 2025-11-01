using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;
using System.IO;

namespace RequestFiend.UI;

public partial class NewRequestTemplateCollectionPage : ContentPage {
    public NewRequestTemplateCollectionModel Model {
        get => BindingContext as NewRequestTemplateCollectionModel ?? throw new InvalidOperationException();
        set => BindingContext = value;
    }

    public NewRequestTemplateCollectionPage() {
        Model = new();
        InitializeComponent();
    }

    private async void OnCreateClicked(object sender, EventArgs e) {
        var collection = new RequestTemplateCollection() {
            Name = Model.Name
        };
        var fileName = $"{string.Concat(Model.Name.Split(Path.GetInvalidFileNameChars()))}.json";
        var stream = new MemoryStream();

        System.Text.Json.JsonSerializer.Serialize(stream, collection);

        var saveResult = await FileSaver.Default.SaveAsync(fileName, stream);

        if (saveResult.IsSuccessful) {
            // TODO move initialization logic to RequestTemplateCollectionPage and use bindingcontext over there
            var newContent = new ShellContent() {
                Title = Model.Name,
                Content = new RequestTemplateCollectionPage(collection, saveResult.FilePath),
                Route = $"RequestTemplateCollection_{Guid.NewGuid()}"
            };

            Shell.Current.Items.Add(newContent);

            await Shell.Current.GoToAsync($"//{newContent.Route}");
        }
        else {
            Toast.Make("Failed to create collection!");
        }
    }
}
