using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class NewRequestTemplatePage : ContentPage<NewRequestTemplateModel> /*, IRecipient<RequestTemplateCollectionUpdatedMessage>*/ {
    public NewRequestTemplatePage(NewRequestTemplateModel model) {
        Model = model;
        InitializeComponent();
        //WeakReferenceMessenger.Default.Register(this, model.filePath);
    }

    //public void Receive(RequestTemplateCollectionUpdatedMessage message) {
    //    if (!Model.Url.IsModified) {
    //        Model.Url.Reset();
    //    }
    //}
}