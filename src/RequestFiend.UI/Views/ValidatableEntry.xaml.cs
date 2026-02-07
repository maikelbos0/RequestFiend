using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.UI.Views;

public partial class ValidatableEntry : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(ValidatableProperty<string?>), typeof(ValidatableEntry), default(ValidatableProperty<string?>));

    public ValidatableProperty<string?> Text {
        get => (ValidatableProperty<string?>)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ValidatableEntry() {
        InitializeComponent();
    }

    private void OnIconTapped(object sender, TappedEventArgs e) {
        Entry.Focus();
    }

    private void OnIconPointerPressed(object sender, PointerEventArgs e) {
        Entry.Focus();
    }
}
