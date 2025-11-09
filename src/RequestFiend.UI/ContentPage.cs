using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using System;

namespace RequestFiend.UI;

public partial class ContentPage<TModel> : ContentPage where TModel : class {
    public TModel Model {
        get => BindingContext as TModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    public void ShowError(string message) {
        this.ShowPopup(new Views.ErrorPopup(message));
    }
}
