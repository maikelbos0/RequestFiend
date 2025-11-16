using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using RequestFiend.Models;
using System;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class ContentPage<TModel> : ContentPage where TModel : ModelBase {
    public TModel Model {
        get => BindingContext as TModel ?? throw new InvalidOperationException();
        init => BindingContext = value;
    }

    protected override void OnSizeAllocated(double width, double height) {
        base.OnSizeAllocated(width, height);
        Model.PageWidth = Width;
    }

    public async Task ShowError(string message) {
        await this.ShowPopupAsync(new Views.ErrorPopup(message));
    }
}
