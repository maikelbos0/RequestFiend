using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : ContentPage {
    private readonly RequestTemplateCollection collection;
    private readonly string filePath;

    public RequestTemplateCollectionModel Model {
        get => BindingContext as RequestTemplateCollectionModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    public RequestTemplateCollectionPage(RequestTemplateCollection collection, string filePath) {
        this.collection = collection;
        this.filePath = filePath;
        Model = new();
        InitializeComponent();
        Title = collection.Name;
    }
}
