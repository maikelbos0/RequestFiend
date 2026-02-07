using System;
using System.Linq;

namespace RequestFiend.Models.PropertyTypes;

public sealed class ValidatableString : ValidatableProperty {
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
    public override bool HasError {
        get => field;
        protected set => SetProperty(ref field, value);
    }
    public override bool IsModified {
        get => field;
        protected set => SetProperty(ref field, value);
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
