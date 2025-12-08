using CommunityToolkit.Maui.Extensions;
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

public partial class AppShell : Shell, IRecipient<SuccessMessage>, IRecipient<ErrorMessage>, IRecipient<OpenCollectionRequestMessage>, IRecipient<OpenTemplateRequestMessage> {
    private CancellationTokenSource? messageCancellationTokenSource;

    public AppShell() {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args) {
        TitleLabel.Text = CurrentItem.Title;
        CloseButton.IsVisible = CurrentItem.StyleId != null;
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

    public async void Receive(ErrorMessage message) {
        await this.ShowPopupAsync(new ErrorPopup(message.Text));
    }

    public async void Receive(OpenCollectionRequestMessage message) {
        var collectionItem = Items.SingleOrDefault(item => string.Equals(item.StyleId, message.FilePath, StringComparison.OrdinalIgnoreCase));

        if (collectionItem == null) {
            collectionItem = new FlyoutItem() {
                Title = Path.GetFileNameWithoutExtension(message.FilePath),
                Icon = "folder_open_solid_full.png",
                Route = $"RequestTemplateCollection_{Guid.NewGuid()}",
                StyleId = message.FilePath
            };

            using (GetRequiredService<IModelDataProvider<(string, RequestTemplateCollection)>>().CreateScope((message.FilePath, message.Collection))) {
                collectionItem.Items.Add(new Tab() {
                    Title = "Collection settings",
                    Icon = "bars_solid_full.png",
                    Items = {
                    new RequestTemplateCollectionPage(GetRequiredService<RequestTemplateCollectionModel>())
                }
                });

                collectionItem.Items.Add(new Tab() {
                    Title = "New request",
                    Icon = "plus_solid_full.png",
                    Items = {
                    new NewRequestTemplatePage(GetRequiredService<NewRequestTemplateModel>())
                }
                });
            }

            foreach (var request in message.Collection.Requests) {
                collectionItem.Items.Add(CreateRequestTab(message.FilePath, message.Collection, request));
            }

            Shell.Current.Items.Add(collectionItem);
        }

        await Shell.Current.GoToAsync($"//{collectionItem.Route}");
    }

    public async void Receive(OpenTemplateRequestMessage message) {
        var collectionItem = Items.Single(item => string.Equals(item.StyleId, message.FilePath, StringComparison.OrdinalIgnoreCase));
        var item = CreateRequestTab(message.FilePath, message.Collection, message.Request);

        collectionItem.Items.Add(item);
        await Shell.Current.GoToAsync($"//{collectionItem.Route}/{item.Route}");
    }

    private Tab CreateRequestTab(string filePath, RequestTemplateCollection collection, RequestTemplate request) {
        using var _ = GetRequiredService<IModelDataProvider<(string, RequestTemplateCollection, RequestTemplate)>>().CreateScope((filePath, collection, request));

        var item = new Tab() {
            Icon = "paper_plane_solid_full.png",
            Title = request.Name,
            Items = {
                new RequestTemplatePage(GetRequiredService<RequestTemplateModel>())
            },
            Route = $"RequestTemplate_{request.Id}"
        };
        WeakReferenceMessenger.Default.Register<Tab, RequestTemplateUpdatedMessage, Guid>(item, request.Id, (tab, message) => tab.Title = request.Name);
        WeakReferenceMessenger.Default.Register<Tab, RequestTemplateDeletedMessage, Guid>(item, request.Id, async (tab, _) => {
            if (tab.Parent is ShellItem collectionItem) {
                collectionItem.Items.Remove(tab);
                await Shell.Current.GoToAsync($"//{collectionItem.Route}");
            }
        });

        return item;
    }

    private T GetRequiredService<T>() where T : notnull
        => (Handler ?? throw new InvalidOperationException()).GetRequiredService<T>();
}
