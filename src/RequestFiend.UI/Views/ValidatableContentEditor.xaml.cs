using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;

namespace RequestFiend.UI.Views;

public partial class ValidatableContentEditor : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(ValidatableProperty<string>),
        typeof(ValidatableContentEditor),
        default(ValidatableProperty<string>),
        propertyChanged: (bindable, _, _) => ((ValidatableContentEditor)bindable).UpdateOverlay()
    );
    public static readonly BindableProperty CollectionProperty = BindableProperty.Create(
        nameof(Collection),
        typeof(RequestTemplateCollection),
        typeof(ValidatableContentEditor),
        default(RequestTemplateCollection),
        propertyChanged: (bindable, _, _) => ((ValidatableContentEditor)bindable).UpdateOverlay()
    );

    public ValidatableProperty<string> Text {
        get => (ValidatableProperty<string>)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public RequestTemplateCollection? Collection {
        get => GetValue(CollectionProperty) as RequestTemplateCollection;
        set => SetValue(CollectionProperty, value);
    }

    public ValidatableContentEditor() {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<ValidatableContentEditor, RequestTemplateCollectionSettingsUpdatedMessage>(this, (_, message) => {
            if (message.Collection == Collection) {
                UpdateOverlay();
            }
        });
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
        UpdateOverlay();
    }

    private void UpdateOverlay() {
        if (Collection != null && Text != null) {
            var variables = Collection.GetVariables();
            var hasVariables = false;

            Overlay.IsVisible = true;
            Overlay.FormattedText = null;
            Overlay.FormattedText = new();

            foreach (var span in VariableService.ProcessText(Text.Value, text => new Span() { Text = text, Style = (Style)Application.Current!.Resources["EditorOverlayText"] }, CreateVariableReferenceSpan)) {
                Overlay.FormattedText.Spans.Add(span);
            }

            Span CreateVariableReferenceSpan(string variableReference) {
                var span = new Span() {
                    Text = variableReference
                };

                if (variables.TryGetValue(variableReference.Trim('{', '}'), out var value)) {
                    hasVariables = true;
                    span.Style = (Style)Application.Current!.Resources["EditorOverlayVariableReference"];
                }
                else {
                    span.Style = (Style)Application.Current!.Resources["EditorOverlayMissingReference"];
                }

                return span;
            }

            if (hasVariables) {
                ToolTipProperties.SetText(Overlay, Collection.ApplyVariables(Text.Value));
            }
            else {
                ToolTipProperties.SetText(Overlay, default!);
            }
        }
    }
}
