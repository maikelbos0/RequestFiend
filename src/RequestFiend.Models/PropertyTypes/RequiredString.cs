using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RequestFiend.Models.PropertyTypes;

// For now we don't need composable validation, but if we do, something like this will be the next step:
// https://learn.microsoft.com/en-us/dotnet/architecture/maui/validation

public class RequiredString : ObservableObject {
    public static implicit operator string(RequiredString requiredString) => requiredString.Value ?? throw new InvalidOperationException();

    private string? value;
    private bool isModified;
    private bool? isValid;
    private readonly Func<string?> defaultValueProvider;

    public string? Value {
        get => value;
        set {
            IsModified = SetProperty(ref this.value, value);
            Validate();
        }
    }

    // TODO decide how to show modified status
    public bool IsModified {
        get => isModified;
        set => SetProperty(ref isModified, value);
    }

    public bool? IsValid {
        get => isValid;
        private set => SetProperty(ref isValid, value);
    }

    public RequiredString() : this(() => null) { }

    public RequiredString(Func<string?> defaultValueProvider) {
        this.defaultValueProvider = defaultValueProvider;
        Reset();
    }

    public bool Validate() {
        var isValid = !string.IsNullOrWhiteSpace(value);
        IsValid = isValid;
        return isValid;
    }

    public void Reset() {
        Value = defaultValueProvider();
        IsModified = false;
        IsValid = null;
    }
}
