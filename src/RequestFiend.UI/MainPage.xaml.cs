using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RequestFiend.UI;

public partial class MainPage : ContentPage {
    public MainPageModel Model {
        get => BindingContext as MainPageModel ?? throw new InvalidOperationException();
        set => BindingContext = value;
    }

    public MainPage() {
        Model = new();
        InitializeComponent();
    }

    private async void OnCreateNewCollectionClicked(object sender, EventArgs e) {
        var collection = new RequestTemplateCollection() {
            Name = Model.NewCollectionName,
            DefaultUrl = Model.NewCollectionDefaultUrl,
            Requests = [
                new() {
                    Name = "Test",
                    Method = "GET",
                    Url = "https://localhost"
                }
            ]
        };
        var fileName = $"{string.Concat(collection.Name.Split(Path.GetInvalidFileNameChars()))}.json";
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, collection);

        var saveResult = await FileSaver.Default.SaveAsync(fileName, stream);

        if (saveResult.IsSuccessful) {
            await RequestTemplateCollectionPage.Open(collection, fileName);
            Model = new();

            return;
        }

        Toast.Make("Failed to create collection.");
    }

    private async void OnOpenExistingCollectionClicked(object sender, EventArgs e) {
        var file = await FilePicker.Default.PickAsync(new() {
            FileTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>() {
                { DevicePlatform.Android, ["application/json"] },
                { DevicePlatform.iOS, ["public.json"] },
                { DevicePlatform.MacCatalyst, ["public.json"] },
                { DevicePlatform.WinUI, ["*.json"] },
            })
        });

        if (file != null) {
            using var stream = await file.OpenReadAsync();
            var collection = await JsonSerializer.DeserializeAsync<RequestTemplateCollection>(stream);

            if (collection != null) {
                await RequestTemplateCollectionPage.Open(collection, file.FullPath);

                return;
            }
        }

        Toast.Make("Failed to load collection.");
    }
}
