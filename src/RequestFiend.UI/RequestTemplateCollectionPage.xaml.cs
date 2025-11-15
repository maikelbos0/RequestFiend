using RequestFiend.Core;
using RequestFiend.Models;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : RequestTemplateCollectionPageBase<RequestTemplateCollectionModel> {
    public RequestTemplateCollectionPage(string filePath, RequestTemplateCollection collection) : base(filePath, collection) {
        Model = new(collection);
        InitializeComponent();
    }

    private async void OnUpdateCollectionClicked(object sender, System.EventArgs e) {
        if (!Model.TryUpdateRequestTemplateCollection(collection)) {
            return;
        }

        await SaveCollection();

        await SuccessMessage.Show("Changes have been saved");
    }

    public Task ShowMessage(string text) => SuccessMessage.Show(text);
}
