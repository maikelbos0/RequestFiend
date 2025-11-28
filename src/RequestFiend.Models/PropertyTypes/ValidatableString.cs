using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RequestFiend.Models.PropertyTypes;

public class ValidatableString : ObservableObject {
    private string? value;
    private bool isModified;
    private bool hasError;
    
    public bool IsRequired { get; }
    public Func<string?> DefaultValueProvider { get; private set; }
    public string? Value {
        get => value;
        set {
            SetProperty(ref this.value, value);
            HasError = IsRequired && string.IsNullOrWhiteSpace(value);
            IsModified = !HasError && value != DefaultValueProvider();
        }
    }
    public bool IsModified {
        get => isModified;
        set => SetProperty(ref isModified, value);
    }
    public bool HasError {
        get => hasError;
        private set => SetProperty(ref hasError, value);
    }

    public ValidatableString(bool isRequired) : this(isRequired, () => null) { }

    public ValidatableString(bool isRequired, Func<string?> defaultValueProvider) {
        IsRequired = isRequired;
        DefaultValueProvider = defaultValueProvider;
        Reset();
    }

    public void Reset() {
        Value = DefaultValueProvider();
    }

    public void Reinitialize(Func<string?> defaultValueProvider) {
        DefaultValueProvider = defaultValueProvider;
        Value = defaultValueProvider();
    }
}
