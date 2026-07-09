using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading.Tasks;

namespace RequestFiend.UI.Views;

public partial class ValidatableEditor : Grid, IRecipient<ActiveEnvironmentChangedMessage>, IRecipient<RequestTemplateCollectionSettingsUpdatedMessage>, IRecipient<ValidatablePropertyUpdatedMessage> {
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
    public static readonly BindableProperty EditorStyleProperty = BindableProperty.Create(
        nameof(EditorStyle),
        typeof(Style),
        typeof(ValidatableEditor),
        default(Style)
    );

    private readonly IEnvironmentService environmentService;

    public ValidatableProperty<string> Text {
        get => (ValidatableProperty<string>)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public RequestTemplateCollection? Collection {
        get => GetValue(CollectionProperty) as RequestTemplateCollection;
        set => SetValue(CollectionProperty, value);
    }

    public Style? EditorStyle {
        get => GetValue(EditorStyleProperty) as Style;
        set => SetValue(EditorStyleProperty, value);
    }

    public ValidatableEditor() {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
        environmentService = App.GetRequiredService<IEnvironmentService>();
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

    private async void UpdateOverlay() {
        if (Collection != null && Text != null) {
            try {
                var variableSnapshot = Collection.CreateVariableSnapshot(await environmentService.GetActiveEnvironment());
                var hasVariables = false;

                Overlay.IsVisible = true;
                Overlay.FormattedText = null;
                Overlay.FormattedText = new();

                foreach (var span in VariableService.ProcessText(Text.Value, text => new Span() { Text = text, Style = (Style)Application.Current!.Resources["EditorOverlayText"] }, CreateVariableReferenceSpan)) {
                    Overlay.FormattedText.Spans.Add(span);
                }

                if (hasVariables) {
                    ToolTipProperties.SetText(Overlay, variableSnapshot.Apply(Text.Value));
                }
                else {
                    ToolTipProperties.SetText(Overlay, default!);
                }

                Span CreateVariableReferenceSpan(string variableReference) {
                    var span = new Span() {
                        Text = variableReference
                    };

                    if (variableSnapshot.Variables.TryGetValue(variableReference, out var value)) {
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

    public async void Receive(ActiveEnvironmentChangedMessage _) {
        await Task.Yield();
        UpdateOverlay();
    }

    public void Receive(RequestTemplateCollectionSettingsUpdatedMessage message) {
        if (message.Collection == Collection) {
            UpdateOverlay();
        }
    }

    public void Receive(ValidatablePropertyUpdatedMessage message) {
        if (message.Property == Text) {
            UpdateOverlay();
        }
    }
}
