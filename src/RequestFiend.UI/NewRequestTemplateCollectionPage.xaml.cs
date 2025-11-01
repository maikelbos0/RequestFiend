using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using System;
using System.IO;

namespace RequestFiend.UI;

public partial class NewRequestTemplateCollectionPage : ContentPage {
    public NewRequestTemplateCollectionPage() {
        BindingContext = new NewRequestTemplateCollection();
        InitializeComponent();        
    }

    private async void OnCreateClicked(object sender, EventArgs e) {
        var context = BindingContext as NewRequestTemplateCollection ?? throw new InvalidOperationException();
        var collection = new RequestTemplateCollection() { 
            Name = context.Name
        };
        var fileName = $"{string.Concat(collection.Name.Split(Path.GetInvalidFileNameChars()))}.json";
        var stream = new MemoryStream();

        System.Text.Json.JsonSerializer.Serialize(stream, collection);

        var saveResult = await FileSaver.Default.SaveAsync(fileName, stream);

        if (saveResult.IsSuccessful) {
            // TODO move initialization logic to RequestTemplateCollectionPage and use bindingcontext over there
            var newContent = new ShellContent() {
                Title = context.Name,
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

// TODO move or something
public class NewRequestTemplateCollection {
    public string Name { get; set; } = "New collection";
}
