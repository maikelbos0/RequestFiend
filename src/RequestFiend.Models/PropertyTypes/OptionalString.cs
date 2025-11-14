using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RequestFiend.Models.PropertyTypes;

public class OptionalString : ObservableObject {
    public static implicit operator string?(OptionalString optionalString) => optionalString.Value;

    private string? value;
    private readonly Func<string?> defaultValueProvider;

    public string? Value {
        get => value;
        set => SetProperty(ref this.value, value);
    }

    public OptionalString() : this(() => null) { }

    public OptionalString(Func<string?> defaultValueProvider) {
        this.defaultValueProvider = defaultValueProvider;
        Reset();
    }

    public void Reset() {
        Value = defaultValueProvider();
    }
}
