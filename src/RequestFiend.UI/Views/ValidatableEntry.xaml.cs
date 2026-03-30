using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.UI.Views;

public partial class ValidatableEntry : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(ValidatableProperty<string>), typeof(ValidatableEntry), default(ValidatableProperty<string>));

    public ValidatableProperty<string> Text {
        get => (ValidatableProperty<string>)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ValidatableEntry() {
        InitializeComponent();
    }

    private void OnOverlayTapped(object sender, TappedEventArgs e) {
        Entry.Focus();
    }

    private void OnOverlayPointerPressed(object sender, PointerEventArgs e) {
        Entry.Focus();
    }

    private void OnEntryFocused(object sender, FocusEventArgs e) {
        Label.IsVisible = false;
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e) {
        Label.IsVisible = true;
    }
}
