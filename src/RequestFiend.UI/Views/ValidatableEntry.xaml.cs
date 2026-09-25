using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using RequestFiend.Models;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Threading.Tasks;

namespace RequestFiend.UI.Views;

public partial class ValidatableEntry : Grid, IRecipient<ActiveEnvironmentChangedMessage>, IRecipient<RequestTemplateCollectionSettingsUpdatedMessage>, IRecipient<ValidatablePropertyUpdatedMessage> {
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(ValidatableProperty<string>),
        typeof(ValidatableEntry),
        default(ValidatableProperty<string>),
        propertyChanged: (bindable, _, _) => ((ValidatableEntry)bindable).UpdateOverlay()
    );
    public static readonly BindableProperty VariableSnapshotProviderProperty = BindableProperty.Create(
        nameof(VariableSnapshotProvider),
        typeof(IVariableSnapshotProvider),
        typeof(ValidatableEntry),
        default(IVariableSnapshotProvider),
        propertyChanged: (bindable, _, _) => ((ValidatableEntry)bindable).UpdateOverlay()
    );
    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPassword),
        typeof(bool),
        typeof(ValidatableEntry),
        false
    );

    public ValidatableProperty<string>? Text {
        get => GetValue(TextProperty) as ValidatableProperty<string>;
        set => SetValue(TextProperty, value);
    }

    public IVariableSnapshotProvider? VariableSnapshotProvider {
        get => GetValue(VariableSnapshotProviderProperty) as IVariableSnapshotProvider;
        set => SetValue(VariableSnapshotProviderProperty, value);
    }

    public bool IsPassword {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public ValidatableEntry() {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    private void OnOverlayTapped(object sender, TappedEventArgs e) {
        Entry.Focus();
    }

    private void OnEntryFocused(object sender, FocusEventArgs e) {
        Overlay.IsVisible = false;
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e) {
        UpdateOverlay();
    }

    private async void UpdateOverlay() {
        if (VariableSnapshotProvider != null && Text != null) {
            try {
                var variableSnapshot = await VariableSnapshotProvider.CreateVariableSnapshot();
                var hasVariables = false;

                Overlay.IsVisible = true;
                Overlay.FormattedText = null;
                Overlay.FormattedText = new();

                foreach (var span in VariableService.ProcessText(Text.Value, text => new Span() { Text = text }, CreateVariableReferenceSpan)) {
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
                        span.Style = (Style)Application.Current!.Resources["VariableReference"];
                    }
                    else {
                        span.Style = (Style)Application.Current!.Resources["MissingReference"];
                    }

                    return span;
                }
            }
            catch { }
        }
        else {
            Overlay.IsVisible = false;
            Overlay.FormattedText = null;
        }
    }

    public async void Receive(ActiveEnvironmentChangedMessage _) {
        await Task.Yield();
        UpdateOverlay();
    }

    public void Receive(RequestTemplateCollectionSettingsUpdatedMessage message) {
        if (message.File == VariableSnapshotProvider?.File) {
            UpdateOverlay();
        }
    }

    public void Receive(ValidatablePropertyUpdatedMessage message) {
        if (message.Property == Text) {
            UpdateOverlay();
        }
    }
}
