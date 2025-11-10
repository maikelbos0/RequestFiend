using RequestFiend.Core;
using RequestFiend.UI.Models;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : RequestTemplateCollectionPageBase<RequestTemplateCollectionModel> {
    public RequestTemplateCollectionPage(string filePath, RequestTemplateCollection collection) : base(filePath, collection) {
        Model = new(collection);
        InitializeComponent();
        Title = collection.Name;
    }

    private async void OnUpdateCollectionClicked(object sender, System.EventArgs e) {
        if (!Model.TryUpdateRequestTemplateCollection(collection)) {
            return;
        }

        await SaveCollection();
        Title = collection.Name;

        // TODO show feedback
    }
}
