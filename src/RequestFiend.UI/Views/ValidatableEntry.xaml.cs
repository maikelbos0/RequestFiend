using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Collections.Generic;

namespace RequestFiend.UI.Views;

public partial class ValidatableEntry : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(ValidatableProperty<string>), typeof(ValidatableEntry), default(ValidatableProperty<string>));
    public static readonly BindableProperty VariablesProperty = BindableProperty.Create(nameof(Variables), typeof(Dictionary<string, string>), typeof(ValidatableEntry), default(Dictionary<string, string>));

    public ValidatableProperty<string> Text {
        get => (ValidatableProperty<string>)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Dictionary<string, string>? Variables {
        get => GetValue(VariablesProperty) as Dictionary<string, string>;
        set => SetValue(VariablesProperty, value);
    }

    public ValidatableEntry() {
        InitializeComponent();
        UpdateOverlay();
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
        if (Overlay.IsVisible = Variables != null) {
            Overlay.FormattedText = null;
            Overlay.FormattedText = new();

            foreach (var span in VariableService.ProcessText(Text.Value, text => new Span() { Text = text }, CreateVariableReferenceSpan)) {
                Overlay.FormattedText.Spans.Add(span);
            }

            Span CreateVariableReferenceSpan(string variableReference) {
                var span = new Span() {
                    Text = variableReference
                };

                if (Variables!.TryGetValue(variableReference.Trim('{', '}'), out var value)) {
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
