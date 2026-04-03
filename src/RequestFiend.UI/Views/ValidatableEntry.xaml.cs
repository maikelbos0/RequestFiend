using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;

namespace RequestFiend.UI.Views;

public partial class ValidatableEntry : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(ValidatableProperty<string>),
        typeof(ValidatableEntry),
        default(ValidatableProperty<string>),
        propertyChanged: (bindable, _, _) => ((ValidatableEntry)bindable).UpdateOverlay()
    );
    public static readonly BindableProperty CollectionProperty = BindableProperty.Create(
        nameof(Collection),
        typeof(RequestTemplateCollection),
        typeof(ValidatableEntry),
        default(RequestTemplateCollection),
        propertyChanged: (bindable, _, _) => ((ValidatableEntry)bindable).UpdateOverlay()
    );

    public ValidatableProperty<string>? Text {
        get => GetValue(TextProperty) as ValidatableProperty<string>;
        set => SetValue(TextProperty, value);
    }

    public RequestTemplateCollection? Collection {
        get => GetValue(CollectionProperty) as RequestTemplateCollection;
        set => SetValue(CollectionProperty, value);
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
        Overlay.IsVisible = false;
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e) {
        UpdateOverlay();
    }

    private void UpdateOverlay() {
        if (Collection != null && Text != null) {
            var variables = Collection.GetVariables();

            Overlay.IsVisible = true;
            Overlay.FormattedText = null;
            Overlay.FormattedText = new();

            foreach (var span in VariableService.ProcessText(Text.Value, text => new Span() { Text = text }, CreateVariableReferenceSpan)) {
                Overlay.FormattedText.Spans.Add(span);
            }

            Span CreateVariableReferenceSpan(string variableReference) {
                var span = new Span() {
                    Text = variableReference
                };

                if (variables.TryGetValue(variableReference.Trim('{', '}'), out var value)) {
                    span.Style = (Style)Application.Current!.Resources["VariableReference"];
                }
                else {
                    span.Style = (Style)Application.Current!.Resources["MissingReference"];
                }

                return span;
            }
        }
    }
}
