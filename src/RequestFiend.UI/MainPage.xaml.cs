using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using RequestFiend.Core;
using RequestFiend.UI.Configuration;
using RequestFiend.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class MainPage : ContentPage<MainPageModel> {
    public MainPage() {
        Model = new();
        InitializeComponent();
    }

    private async void OnCreateNewCollectionClicked(object sender, EventArgs e) {
        if (!Model.NewRequestTemplateCollection.TryCreateRequestTemplateCollection(out var collection)) {
            return;
        }
        
        var fileName = $"{string.Concat(collection.Name.Split(Path.GetInvalidFileNameChars()))}.json";
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, collection);

        var saveResult = await FileSaver.Default.SaveAsync(fileName, stream);

        if (saveResult.IsSuccessful) {
            await OpenCollection(collection, saveResult.FilePath);
            Model.NewRequestTemplateCollection.Reset();

            return;
        }

        ShowError("Failed to create collection.");
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
            await OpenCollectionFromFile(file.FullPath);
        }
    }

    private async void OnOpenRecentCollectionClicked(object sender, EventArgs e) {
        var filePath = ((RecentCollectionModel)((Button)sender).BindingContext).FilePath;

        await OpenCollectionFromFile(filePath);
    }

    private async Task OpenCollectionFromFile(string filePath) {
        if (File.Exists(filePath)) {
            try {
                var collection = JsonSerializer.Deserialize<RequestTemplateCollection>(File.ReadAllText(filePath));

                if (collection != null) {
                    await OpenCollection(collection, filePath);
                }
                else {
                    ShowError("Failed to load collection.");
                }
            }
            catch (Exception ex) {
                ShowError($"Failed to load collection: {ex.Message}");
            }
        }
        else {
            ShowError("Collection file does not exist.");
            Model.RecentCollections = RecentCollections.Remove(filePath);
        }
    }

    private async Task OpenCollection(RequestTemplateCollection collection, string filePath) {
        var item = Shell.Current.Items.FirstOrDefault(item => string.Equals(item.StyleId, filePath, StringComparison.OrdinalIgnoreCase));

        if (item == null) {
            item = new FlyoutItem() {
                Title = collection.Name,
                Icon = "folder_open_solid_full.png",
                Route = $"RequestTemplateCollection_{Guid.NewGuid()}",
                StyleId = filePath
            };

            item.Items.Add(new Tab() {
                Title = "Collection settings",
                Icon = "bars_solid_full.png",
                Items = {
                    new RequestTemplateCollectionPage(filePath, collection, item)
                }
            });

            item.Items.Add(new Tab() {
                Title = "New request",
                Icon = "plus_solid_full.png",
                Items = {
                    new NewRequestTemplatePage(filePath, collection, item)
                }
            });

            foreach (var request in collection.Requests) {
                item.Items.Add(new Tab() {
                    Icon = "paper_plane_solid_full.png",
                    Title = request.Name,
                    Items = {
                        new RequestTemplatePage(filePath, collection, request)
                    }
                });
            }

            Shell.Current.Items.Add(item);
            Model.RecentCollections = RecentCollections.Add(filePath);
        }

        await Shell.Current.GoToAsync($"//{item.Route}");
    }
}
