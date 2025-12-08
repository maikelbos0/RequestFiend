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
using System.Threading.Tasks;

namespace RequestFiend.UI;

public static class ShellExtensions {
    public static T GetRequiredService<T>(this Shell shell) where T : notnull
        => (shell.Handler ?? throw new InvalidOperationException()).GetRequiredService<T>();

    public static async Task OpenCollection(this Shell shell, string filePath, RequestTemplateCollection collection) {
        var collectionItem = Shell.Current.Items.SingleOrDefault(item => string.Equals(item.StyleId, filePath, StringComparison.OrdinalIgnoreCase));

        if (collectionItem == null) {
            collectionItem = new FlyoutItem() {
                Title = Path.GetFileNameWithoutExtension(filePath),
                Icon = "folder_open_solid_full.png",
                Route = $"RequestTemplateCollection_{Guid.NewGuid()}",
                StyleId = filePath
            };

            // TODO register pages in service collection?
            using (shell.GetRequiredService<IModelDataProvider<(string, RequestTemplateCollection)>>().CreateScope((filePath, collection))) {
                collectionItem.Items.Add(new Tab() {
                    Title = "Collection settings",
                    Icon = "bars_solid_full.png",
                    Items = {
                    new RequestTemplateCollectionPage(shell.GetRequiredService<RequestTemplateCollectionModel>())
                }
                });
            }

            collectionItem.Items.Add(new Tab() {
                Title = "New request",
                Icon = "plus_solid_full.png",
                Items = {
                    new NewRequestTemplatePage(filePath, collection)
                }
            });

            foreach (var request in collection.Requests) {
                collectionItem.Items.Add(CreateRequestTab(shell, filePath, collection, request));
            }

            Shell.Current.Items.Add(collectionItem);
        }

        await Shell.Current.GoToAsync($"//{collectionItem.Route}");
    }

    public static async Task OpenRequest(this Shell shell, string filePath, RequestTemplateCollection collection, RequestTemplate request) {
        var collectionItem = shell.Items.Single(item => string.Equals(item.StyleId, filePath, StringComparison.OrdinalIgnoreCase));
        var item = CreateRequestTab(shell, filePath, collection, request);

        collectionItem.Items.Add(item);
        await Shell.Current.GoToAsync($"//{collectionItem.Route}/{item.Route}");
    }

    private static Tab CreateRequestTab(this Shell shell, string filePath, RequestTemplateCollection collection, RequestTemplate request) {
        using var _ = shell.GetRequiredService<IModelDataProvider<(string, RequestTemplateCollection, RequestTemplate)>>().CreateScope((filePath, collection, request));

        var item = new Tab() {
            Icon = "paper_plane_solid_full.png",
            Title = request.Name,
            Items = {
                new RequestTemplatePage(shell.GetRequiredService<RequestTemplateModel>())
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
}
