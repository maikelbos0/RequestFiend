using Microsoft.Maui.Controls;
using RequestFiend.Models;
using System;

namespace RequestFiend.UI.Views;

public partial class ContentPage<TModel> : ContentPage where TModel : BoundModelBase {
    public TModel Model {
        get => BindingContext as TModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    protected override void OnSizeAllocated(double width, double height) {
        base.OnSizeAllocated(width, height);
        Model.PageWidth = Width;
    }
}
