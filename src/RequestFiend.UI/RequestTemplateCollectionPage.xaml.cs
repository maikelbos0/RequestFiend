using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : RequestTemplateCollectionPageBase<RequestTemplateCollectionModel> {
    private readonly ShellItem parentItem;

    public RequestTemplateCollectionPage(string filePath, RequestTemplateCollection collection, ShellItem parentItem) : base(filePath, collection) {
        Model = new(collection);
        InitializeComponent();
        Title = collection.Name;
        this.parentItem = parentItem;
    }

    private async void OnUpdateCollectionClicked(object sender, System.EventArgs e) {
        if (!Model.TryUpdateRequestTemplateCollection(collection)) {
            return;
        }

        await SaveCollection();
        Title = collection.Name;
        parentItem.Title = collection.Name;
        ((AppShell)Shell.Current).SetTitle(collection.Name);

        // TODO show feedback
    }
}
