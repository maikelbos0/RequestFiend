using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;

namespace RequestFiend.UI.Views;

public partial class ValidatableEditor : AbsoluteLayout {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(ValidatableProperty<string>),
        typeof(ValidatableEditor),
        default(ValidatableProperty<string>),
        propertyChanged: (bindable, _, _) => ((ValidatableEditor)bindable).UpdateOverlay()
    );
    public static readonly BindableProperty CollectionProperty = BindableProperty.Create(
        nameof(Collection),
        typeof(RequestTemplateCollection),
        typeof(ValidatableEditor),
        default(RequestTemplateCollection),
        propertyChanged: (bindable, _, _) => ((ValidatableEditor)bindable).UpdateOverlay()
    );

    public ValidatableProperty<string> Text {
        get => (ValidatableProperty<string>)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public RequestTemplateCollection? Collection {
        get => GetValue(CollectionProperty) as RequestTemplateCollection;
        set => SetValue(CollectionProperty, value);
    }

    public ValidatableEditor() {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<ValidatableEditor, RequestTemplateCollectionSettingsUpdatedMessage>(this, (_, message) => {
            if (message.Collection == Collection) {
                UpdateOverlay();
            }
        });
        WeakReferenceMessenger.Default.Register<ValidatableEditor, ValidatablePropertyUpdatedMessage>(this, (_, message) => {
            if (message.Property == Text) {
                UpdateOverlay();
            }
        });
    }

    private void OnOverlayTapped(object sender, TappedEventArgs e) {
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
            try {
                var variables = Collection.GetVariables();
                var hasVariables = false;

                Overlay.IsVisible = true;
                Overlay.FormattedText = null;
                Overlay.FormattedText = new();

                foreach (var span in VariableService.ProcessText(Text.Value, text => new Span() { Text = text, Style = (Style)Application.Current!.Resources["EditorOverlayText"] }, CreateVariableReferenceSpan)) {
                    Overlay.FormattedText.Spans.Add(span);
                }

                if (hasVariables) {
                    ToolTipProperties.SetText(Overlay, Collection.ApplyVariables(Text.Value));
                }
                else {
                    ToolTipProperties.SetText(Overlay, default!);
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

            }
            catch { }
        }
    }
}
