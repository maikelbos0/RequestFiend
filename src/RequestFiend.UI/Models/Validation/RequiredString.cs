using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RequestFiend.UI.Models.Validation;

// For now we don't need composable validation, but if we do, something like this will be the next step:
// https://learn.microsoft.com/en-us/dotnet/architecture/maui/validation

public class RequiredString : ObservableObject {
    public static implicit operator string(RequiredString requiredValue) => requiredValue.Value ?? throw new InvalidOperationException();
    public static implicit operator RequiredString(string? requiredValue) => new(requiredValue);

    private string? value;
    private bool? isValid;

    public string? Value {
        get => value;
        set {
            SetProperty(ref this.value, value);
            Validate();
        }
    }
    public bool? IsValid {
        get => isValid;
        private set => SetProperty(ref isValid, value);
    }

    public RequiredString() : this(null) { }

    public RequiredString(string? value) {
        this.value = value;
    }

    public bool Validate() {
        var isValid = !string.IsNullOrWhiteSpace(value);
        IsValid = isValid;
        return isValid;
    }

    public void Reset() => Reset(null);

    public void Reset(string? value) {
        Value = value;
        IsValid = null;
    }
}
