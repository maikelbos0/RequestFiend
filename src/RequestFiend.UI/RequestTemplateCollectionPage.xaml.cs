using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : ContentPage {
    public RequestTemplateCollectionModel Model {
        get => BindingContext as RequestTemplateCollectionModel ?? throw new InvalidOperationException();
        set => BindingContext = value;
    }

    public RequestTemplateCollectionPage(RequestTemplateCollection collection, string filePath) {
        InitializeComponent();
        Model = new() {
            Collection = collection,
            FilePath = filePath
        };
        Title = collection.Name;
    }
}
