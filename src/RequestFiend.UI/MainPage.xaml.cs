using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using RequestFiend.Core;
using RequestFiend.UI.Configuration;
using RequestFiend.UI.Messages;
using RequestFiend.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class MainPage : ContentPage<MainPageModel>, IRecipient<RequestTemplateCollectionUpdatedMessage> {
    public MainPage() {
        Model = new();
        InitializeComponent();
        WeakReferenceMessenger.Default.Register(this);
    }

    private async void OnCreateNewCollectionClicked(object sender, EventArgs e) {
        var collection = new RequestTemplateCollection();
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, collection);

        var saveResult = await FileSaver.Default.SaveAsync(".json", stream);

        if (saveResult.IsSuccessful) {
            await OpenCollection(collection, saveResult.FilePath);
        }
        else {
            ShowError("Failed to create collection.");
        }
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
        var collectionItem = Shell.Current.Items.FirstOrDefault(item => string.Equals(item.StyleId, filePath, StringComparison.OrdinalIgnoreCase));

        if (collectionItem == null) {
            collectionItem = new FlyoutItem() {
                Title = Path.GetFileNameWithoutExtension(filePath),
                Icon = "folder_open_solid_full.png",
                Route = $"RequestTemplateCollection_{Guid.NewGuid()}",
                StyleId = filePath
            };

            collectionItem.Items.Add(new Tab() {
                Title = "Collection settings",
                Icon = "bars_solid_full.png",
                Items = {
                    new RequestTemplateCollectionPage(filePath, collection)
                }
            });

            collectionItem.Items.Add(new Tab() {
                Title = "New request",
                Icon = "plus_solid_full.png",
                Items = {
                    new NewRequestTemplatePage(filePath, collection, collectionItem)
                }
            });

            foreach (var request in collection.Requests) {
                var item = new Tab() {
                    Icon = "paper_plane_solid_full.png",
                    Title = request.Name,
                    Items = {
                        new RequestTemplatePage(filePath, collection, request)
                    }
                };
                WeakReferenceMessenger.Default.Register<Tab, RequestTemplateUpdatedMessage, Guid>(item, request.Id, (tab, message) => tab.Title = request.Name);
                collectionItem.Items.Add(item);
            }

            Shell.Current.Items.Add(collectionItem);
            Model.RecentCollections = RecentCollections.Push(filePath);
        }

        await Shell.Current.GoToAsync($"//{collectionItem.Route}");
    }

    public void Receive(RequestTemplateCollectionUpdatedMessage message) {
        Model.RecentCollections = RecentCollections.Push(message.FilePath);
    }
}
