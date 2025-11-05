using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : ContentPage {
    public RequestTemplateCollectionModel Model {
        get => BindingContext as RequestTemplateCollectionModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    public RequestTemplateCollectionPage(RequestTemplateCollection collection, string filePath) {
        Model = new() {
            Collection = collection,
            FilePath = filePath
        };
        InitializeComponent();
        Title = collection.Name;
    }
}
