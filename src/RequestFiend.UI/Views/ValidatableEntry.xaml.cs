using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;

namespace RequestFiend.UI.Views;

public partial class ValidatableEntry : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(Text), typeof(ValidatableEntry), default(Text));

    public Text Text {
        get => (Text)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ValidatableEntry() {
        InitializeComponent();
    }
}