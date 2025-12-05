using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Messages;
using RequestFiend.UI.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.UI.Views;

public partial class MainPage : ContentPage<MainPageModel>, IRecipient<RequestTemplateCollectionUpdatedMessage> {
    public MainPage() {
        Model = new() {
            RecentCollections = RecentCollections.Get()
        };
        InitializeComponent();
        WeakReferenceMessenger.Default.Register(this);
    }

    private async void OnCreateNewCollectionClicked(object sender, EventArgs e) {
        var collection = new RequestTemplateCollection();
        var stream = new MemoryStream();

        JsonSerializer.Serialize(stream, collection);

        var saveResult = await FileSaver.Default.SaveAsync(".json", stream);

        if (saveResult.IsSuccessful) {
            await Shell.Current.OpenCollection(saveResult.FilePath, collection);
            Model.RecentCollections = RecentCollections.Push(saveResult.FilePath);
        }
        else if (saveResult.Exception != null) {
            WeakReferenceMessenger.Default.Send(new ErrorMessage($"Failed to create collection: {saveResult.Exception.Message}"));
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
                    await Shell.Current.OpenCollection(filePath, collection);
                    Model.RecentCollections = RecentCollections.Push(filePath);
                }
                else {
                    WeakReferenceMessenger.Default.Send(new ErrorMessage("Failed to load collection."));
                }
            }
            catch (Exception ex) {
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Failed to load collection: {ex.Message}"));
            }
        }
        else {
            WeakReferenceMessenger.Default.Send(new ErrorMessage("Collection file does not exist."));
            Model.RecentCollections = RecentCollections.Remove(filePath);
        }
    }

    public void Receive(RequestTemplateCollectionUpdatedMessage message) {
        Model.RecentCollections = RecentCollections.Push(message.FilePath);
    }   
}
