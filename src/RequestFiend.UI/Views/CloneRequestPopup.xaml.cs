using CommunityToolkit.Maui.Views;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Services;

namespace RequestFiend.UI.Views;

public partial class CloneRequestPopup : Popup {
    public CloneRequestPopup(IRequestTemplateCollectionService requestTemplateCollectionService, IMessageService messageService, FileModel file, RequestTemplateCollection collection, RequestTemplate request) {
        BindingContext = new CloneRequestTemplateModel(CloseAsync, requestTemplateCollectionService, messageService, file, collection, request);
        InitializeComponent();
    }

    public async void OnCancelClicked(object sender, System.EventArgs e) {
        await CloseAsync();
    }
}
