using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class NewRequestTemplatePage : ContentPage {
    public NewRequestTemplateModel Model {
        get => BindingContext as NewRequestTemplateModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    public NewRequestTemplatePage(RequestTemplateCollection collection) {
        Model = new() {
            Collection = collection,
            Url = collection.DefaultUrl
        };
        InitializeComponent();
    }
}