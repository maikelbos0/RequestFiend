using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using RequestFiend.UI.Views;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class AppShell : Shell, IRecipient<SuccessMessage>, IRecipient<OpenCollectionRequestMessage>, IRecipient<OpenTemplateRequestMessage> {
    private CancellationTokenSource? messageCancellationTokenSource;

    public AppShell() {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    private async void OnCloseClicked(object sender, EventArgs e) {
        var currentItem = CurrentItem;
        await GoToAsync("//MainPage");
        Items.Remove(currentItem);
    }

    public async void Receive(SuccessMessage message) {
        this.messageCancellationTokenSource?.Cancel();

        var messageCancellationTokenSource = this.messageCancellationTokenSource = new();

        SuccessLabel.Text = message.Text;
        SuccessLabel.Opacity = 1;
        SuccessLabel.IsVisible = true;

        await Task.Delay(1000);

        for (var i = 0; i < 25; i++) {
            if (messageCancellationTokenSource.Token.IsCancellationRequested) {
                return;
            }

            SuccessLabel.Opacity = (25 - i) / 25.0;
            await Task.Delay(40);
        }

        SuccessLabel.IsVisible = false;
    }

    public async void Receive(OpenCollectionRequestMessage message) {
        var collectionItem = Items.SingleOrDefault(item => string.Equals(item.StyleId, message.FilePath, StringComparison.OrdinalIgnoreCase));

        if (collectionItem == null) {
            using var _ = GetRequiredService<IModelDataProvider>().CreateScope(new RequestTemplateCollectionFileModel(message.FilePath), message.Collection);
            var collectionModel = GetRequiredService<RequestTemplateCollectionModel>();

            collectionItem = new FlyoutItem() {
                Title = Path.GetFileNameWithoutExtension(message.FilePath),
                Icon = "folder_open_solid_full.png",
                Route = $"RequestTemplateCollection_{Guid.NewGuid()}",
                StyleId = message.FilePath
            };

            collectionItem.Items.Add(new Tab() {
                Icon = "bars_solid_full.png",
                Items = {
                    new RequestTemplateCollectionSettingsPage(collectionModel.Settings)
                }
            });

            collectionItem.Items.Add(new Tab() {
                Icon = "plus_solid_full.png",
                Items = {
                    new NewRequestTemplatePage(collectionModel.NewRequest)
                }
            });

            foreach (var request in message.Collection.Requests) {
                collectionItem.Items.Add(CreateRequestTab(request));
            }

            Items.Add(collectionItem);
        }

        await GoToAsync($"//{collectionItem.Route}");
    }

    private Tab CreateRequestTab(RequestTemplate request) {
        using var _ = GetRequiredService<IModelDataProvider>().CreateScope(request);

        var item = new Tab() {
            Icon = "paper_plane_solid_full.png",
            Items = {
                GetRequiredService<RequestTemplatePage>()
            },
            Route = $"RequestTemplate_{request.Id}"
        };

        WeakReferenceMessenger.Default.Register<Tab, RequestTemplateDeletedMessage, Guid>(item, request.Id, async (tab, _) => {
            if (tab.Parent is ShellItem collectionItem) {
                collectionItem.Items.Remove(tab);
                await GoToAsync($"//{collectionItem.Route}");
            }
        });

        return item;
    }

    private T GetRequiredService<T>() where T : notnull
        => (Handler ?? throw new InvalidOperationException()).GetRequiredService<T>();
    
    public async void Receive(OpenTemplateRequestMessage message) {
        using var _ = GetRequiredService<IModelDataProvider>().CreateScope(new RequestTemplateCollectionFileModel(message.FilePath), message.Collection);

        var collectionItem = Items.Single(item => string.Equals(item.StyleId, message.FilePath, StringComparison.OrdinalIgnoreCase));
        var item = CreateRequestTab(message.Request);

        collectionItem.Items.Add(item);
        await GoToAsync($"//{collectionItem.Route}/{item.Route}");
    }
}
