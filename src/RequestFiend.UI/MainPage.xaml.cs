using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;
using System.IO;

namespace RequestFiend.UI;

public partial class MainPage : ContentPage {
    public MainPageModel Model {
        get => BindingContext as MainPageModel ?? throw new InvalidOperationException();
        set => BindingContext = value;
    }

    public MainPage() {
        InitializeComponent();
        Model = new();
    }


    private async void OnCreateNewCollectionClicked(object sender, EventArgs e) {
        var collection = new RequestTemplateCollection() {
            Name = Model.NewCollectionName
        };
        var fileName = $"{string.Concat(collection.Name.Split(Path.GetInvalidFileNameChars()))}.json";
        var stream = new MemoryStream();

        System.Text.Json.JsonSerializer.Serialize(stream, collection);

        var saveResult = await FileSaver.Default.SaveAsync(fileName, stream);

        if (saveResult.IsSuccessful) {
            // TODO move initialization logic to RequestTemplateCollectionPage and use bindingcontext over there
            var newContent = new ShellContent() {
                Title = collection.Name,
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
