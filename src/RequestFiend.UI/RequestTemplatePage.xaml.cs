using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.UI.Models;
using System;

namespace RequestFiend.UI;

public partial class RequestTemplatePage : ContentPage {
    public RequestTemplateModel Model {
        get => BindingContext as RequestTemplateModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    public RequestTemplatePage(RequestTemplate request) {
        Model = new() {
            Request = request
        };
        Title = request.Name;
        InitializeComponent();
    }
}
