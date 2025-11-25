using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RequestFiend.Models.PropertyTypes;

public class Text : ObservableObject {
    public static implicit operator string?(Text text) => text.Value;

    private string? initialValue;
    private string? value;
    private bool isModified;
    private bool hasError;
    
    public bool IsRequired { get; }

    public Func<string?> DefaultValueProvider { get; }

    public string? Value {
        get => value;
        set {
            SetProperty(ref this.value, value);
            HasError = IsRequired && string.IsNullOrWhiteSpace(value);
            IsModified = !HasError && value != initialValue;
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

    public Text(bool isRequired) : this(isRequired, () => null) { }

    public Text(bool isRequired, Func<string?> defaultValueProvider) {
        IsRequired = isRequired;
        DefaultValueProvider = defaultValueProvider;
        Reset();
    }

    public void Reset() {
        Value = initialValue = DefaultValueProvider();
    }
}
