using Microsoft.Maui.Controls;
using RequestFiend.Models;
using System;
using System.ComponentModel;

namespace RequestFiend.UI.Views;

public partial class ContentPage : Microsoft.Maui.Controls.ContentPage {
    public ContentPage(BoundModelBase model) {
        BindingContext = model;

        ParentChanged += UpdateParentTitleAndRegisterNext;
        model.PropertyChanged += UpdateAncestorTitles;

        void UpdateParentTitleAndRegisterNext(object? sender, EventArgs e) {
            var current = (Element)sender!;

            if (current is not Tab && current.Parent is BaseShellItem shellItem) {
                shellItem.Title = model.ShellItemTitle;
                shellItem.ParentChanged += UpdateParentTitleAndRegisterNext;
            }

            current.ParentChanged -= UpdateParentTitleAndRegisterNext;
        }

        void UpdateAncestorTitles(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(BoundModelBase.ShellItemTitle)) {
                Element item = this;

                while (item is not Tab && item.Parent is BaseShellItem shellItem) {
                    shellItem.Title = model.ShellItemTitle;
                    item = item.Parent;
                }
            }
        }
    }

    protected override void OnSizeAllocated(double width, double height) {
        base.OnSizeAllocated(width, height);

        ((BoundModelBase)BindingContext).PageWidth = Width;
    }
}
