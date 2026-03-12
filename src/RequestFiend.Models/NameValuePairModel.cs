using CommunityToolkit.Mvvm.ComponentModel;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.ComponentModel;

namespace RequestFiend.Models;

public class NameValuePairModel : ObservableObject {
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Value { get; }
    public bool HasError { get => field; set => SetProperty(ref field, value); }
    public bool IsModified { get => field; set => SetProperty(ref field, value); }

    public NameValuePairModel() : this(() => "", () => "") { }

    public NameValuePairModel(NameValuePair pair) : this(() => pair.Name, () => pair.Value) { }

    private NameValuePairModel(Func<string> nameProvider, Func<string> valueProvider) {
        Name = new(nameProvider, Validator.Required);
        Value = new(valueProvider);
            
        Name.PropertyChanged += OnValidatablePropertyChanged;
        Value.PropertyChanged += OnValidatablePropertyChanged;

        UpdateState();
    }

    public void Reset(NameValuePair pair) {
        Name.Reset(() => pair.Name);
        Value.Reset(() => pair.Value);
    }

    private void OnValidatablePropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ValidatableProperty.IsModified) || e.PropertyName == nameof(ValidatableProperty.HasError)) {
            UpdateState();
        }
    }

    private void UpdateState() {
        HasError = Name.HasError || Value.HasError;
        IsModified = Name.IsModified || Value.IsModified;
    }
}
