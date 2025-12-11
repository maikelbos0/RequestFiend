using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;

namespace RequestFiend.Models.PropertyTypes;

public class ValidatableString : ObservableObject {
    public ValidationMode Mode { get; }
    public Func<string?> DefaultValueProvider { get; private set; }
    public string? Value {
        get => field;
        set {
            SetProperty(ref field, value);
            HasError = Mode switch {
                ValidationMode.None => false,
                ValidationMode.Required => string.IsNullOrEmpty(value),
                ValidationMode.Numeric => string.IsNullOrEmpty(value) || value.Any(c => !char.IsAsciiDigit(c)),
                _ => throw new NotImplementedException($"Missing implementation for ValidationMode {Mode}")
            };
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

    public ValidatableString(ValidationMode mode) : this(mode, () => null) { }

    public ValidatableString(ValidationMode mode, Func<string?> defaultValueProvider) {
        Mode = mode;
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
