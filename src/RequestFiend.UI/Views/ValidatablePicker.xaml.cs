using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;
using System.Collections.Generic;

namespace RequestFiend.UI.Views;

public partial class ValidatablePicker : AbsoluteLayout {
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(ValidatableProperty<string>), typeof(ValidatablePicker), default(ValidatableProperty<string>));
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<string>), typeof(ValidatablePicker), default(IEnumerable<string>));

    public ValidatableProperty<string> SelectedItem {
        get => (ValidatableProperty<string>)GetValue(SelectedItemProperty);
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
        Picker.IsOpen = true;
    }

    private void OnIconPointerPressed(object sender, PointerEventArgs e) {
        Picker.IsOpen = true;
    }
}
