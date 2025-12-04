using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.Models.Messages;
using RequestFiend.Models.Services;
using System;

namespace RequestFiend.UI.Views;

public partial class NewRequestTemplatePage : RequestTemplateCollectionPageBase<NewRequestTemplateModel>, IRecipient<RequestTemplateCollectionUpdatedMessage> {
    public NewRequestTemplatePage(string filePath, RequestTemplateCollection collection) : base(filePath, collection) {
        Model = new(Shell.Current.GetRequiredService<IFileService>(), filePath, collection);
        InitializeComponent();
        WeakReferenceMessenger.Default.Register(this, filePath);
    }

    private async void OnCreateRequestTemplateClicked(object sender, EventArgs e) {
        if (!Model.TryCreateRequestTemplate(out var request)) {
            return;
        }

        collection.Requests.Add(request);
        await SaveCollection();
        await Shell.Current.OpenRequest(filePath, collection, request);

        Model.Reset();
    }

    public void Receive(RequestTemplateCollectionUpdatedMessage message) {
        if (!Model.Url.IsModified) {
            Model.Url.Reset();
        }
    }
}