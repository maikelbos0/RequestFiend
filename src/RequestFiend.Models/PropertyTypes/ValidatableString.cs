using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RequestFiend.Models.PropertyTypes;

public class ValidatableString : ObservableObject {
    public bool IsRequired { get; }
    public Func<string?> DefaultValueProvider { get; private set; }
    public string? Value {
        get => field;
        set {
            SetProperty(ref field, value);
            HasError = IsRequired && string.IsNullOrWhiteSpace(value);
            IsModified = !HasError && value != DefaultValueProvider();
        }
    }
    public bool IsModified {
        get => field;
        private set => SetProperty(ref field, value);
    }
    public bool HasError {
        get => field;
        private set => SetProperty(ref field, value);
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
