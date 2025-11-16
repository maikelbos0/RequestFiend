using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class ContentPage<TModel> : ContentPage where TModel : class {
    public TModel Model {
        get => BindingContext as TModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    public async Task ShowError(string message) {
        await this.ShowPopupAsync(new Views.ErrorPopup(message));
    }
}
