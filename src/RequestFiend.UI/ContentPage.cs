using Microsoft.Maui.Controls;
using System;

namespace RequestFiend.UI;

public class ContentPage<TModel> : ContentPage where TModel : class {
    public TModel Model {
        get => BindingContext as TModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }
}
