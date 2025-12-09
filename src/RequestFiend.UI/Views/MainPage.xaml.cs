using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Models;
using RequestFiend.Models.Messages;
using RequestFiend.UI.Configuration;

namespace RequestFiend.UI.Views;

public partial class MainPage : ContentPage<MainPageModel>, IRecipient<RequestTemplateCollectionUpdatedMessage> {
    public MainPage(MainPageModel model) {
        Model = model;
        InitializeComponent();
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(RequestTemplateCollectionUpdatedMessage message) {
        Model.RecentCollections = RecentCollections.Push(message.FilePath);
    }   
}
