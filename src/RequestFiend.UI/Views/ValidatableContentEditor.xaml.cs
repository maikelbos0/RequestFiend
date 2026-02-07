using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.UI.Views;

public partial class ValidatableContentEditor : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(ValidatableProperty<string?>), typeof(ValidatableContentEditor), default(ValidatableProperty<string?>));

    public ValidatableProperty<string?> Text {
        get => (ValidatableProperty<string?>)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ValidatableContentEditor() {
        InitializeComponent();
    }

    private void OnIconTapped(object sender, TappedEventArgs e) {
        Editor.Focus();
    }

    private void OnIconPointerPressed(object sender, PointerEventArgs e) {
        Editor.Focus();
    }
}
