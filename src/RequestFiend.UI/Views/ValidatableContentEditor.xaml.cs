using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.UI.Views;

public partial class ValidatableContentEditor : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(ValidatableProperty<string>), typeof(ValidatableContentEditor), default(ValidatableProperty<string>));

    public ValidatableProperty<string> Text {
        get => (ValidatableProperty<string>)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ValidatableContentEditor() {
        InitializeComponent();
        Overlay.IsVisible = true;
    }

    private void OnOverlayTapped(object sender, TappedEventArgs e) {
        Editor.Focus();
    }

    private void OnOverlayPointerPressed(object sender, PointerEventArgs e) {
        Editor.Focus();
    }

    private void OnEditorFocused(object sender, FocusEventArgs e) {
        Overlay.IsVisible = false;
    }

    private void OnEditorUnfocused(object sender, FocusEventArgs e) {
        Overlay.IsVisible = true;
    }
}
