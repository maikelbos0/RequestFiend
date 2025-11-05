using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class RequestTemplatePage : ContentPage {
    private readonly RequestTemplateCollection collection;

    public RequestTemplateModel Model {
        get => BindingContext as RequestTemplateModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    public RequestTemplatePage(RequestTemplateCollection collection, RequestTemplate request) {
        this.collection = collection;
        Model = new() {
            Request = request
        };
        InitializeComponent();
        Title = collection.Name;
    }
}
