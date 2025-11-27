using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;
using System.Collections.Generic;

namespace RequestFiend.UI.Views;

public partial class ValidatablePicker : AbsoluteLayout {
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(ValidatableString), typeof(ValidatablePicker), default(ValidatableString));
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<string>), typeof(ValidatablePicker), default(IEnumerable<string>));

    public ValidatableString SelectedItem {
        get => (ValidatableString)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public IEnumerable<string> ItemsSource {
        get => (IEnumerable<string>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ValidatablePicker() {
        InitializeComponent();
    }

    private void OnIconTapped(object sender, TappedEventArgs e) {
        // Maybe in .net 10 it will be possible to open it; see https://github.com/dotnet/maui/issues/8945
        Picker.Focus();
    }

    private void OnIconPointerPressed(object sender, PointerEventArgs e) {
        // Maybe in .net 10 it will be possible to open it; see https://github.com/dotnet/maui/issues/8945
        Picker.Focus();
    }
}