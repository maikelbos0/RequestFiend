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
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class MainPage : ContentPage {
    public NewRequestTemplateCollectionModel Model {
        get => BindingContext as NewRequestTemplateCollectionModel ?? throw new InvalidOperationException();
        set => BindingContext = value;
    }

    public MainPage() {
        Model = new();
        InitializeComponent();
    }

    private async void OnCreateNewCollectionClicked(object sender, EventArgs e) {
        if (!Model.TryCreateRequestTemplateCollection(out var collection)) {
            return;
        }

        collection.Requests.Add(new() {
            Name = "Test",
            Method = "GET",
            Url = "https://localhost"
        });

        var fileName = $"{string.Concat(collection.Name.Split(Path.GetInvalidFileNameChars()))}.json";
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, collection);

        var saveResult = await FileSaver.Default.SaveAsync(fileName, stream);

        if (saveResult.IsSuccessful) {
            await OpenCollection(collection, fileName);
            Model.Reset();

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
                await OpenCollection(collection, file.FullPath);

                return;
            }
        }

        Toast.Make("Failed to load collection.");
    }

    private static async Task OpenCollection(RequestTemplateCollection collection, string filePath) {
        var item = new FlyoutItem() {
            Title = collection.Name,
            Icon = "folder_open_solid_full.png",
            Route = $"RequestTemplateCollection_{Guid.NewGuid()}"
        };

        item.Items.Add(new Tab() {
            Title = "Collection settings",
            Icon = "bars_solid_full.png",
            Items = {
                new RequestTemplateCollectionPage(collection, filePath)
            }
        });

        item.Items.Add(new Tab() {
            Title = "New request",
            Icon = "plus_solid_full.png",
            Items = {
                new NewRequestTemplatePage(item, collection)
            }
        });

        foreach (var request in collection.Requests) {
            item.Items.Add(new Tab() {
                Icon = "paper_plane_solid_full.png",
                Title = request.Name,
                Items = {
                    new RequestTemplatePage(collection, request)
                }
            });
        }

        Shell.Current.Items.Add(item);
        await Shell.Current.GoToAsync($"//{item.Route}");
    }
}
