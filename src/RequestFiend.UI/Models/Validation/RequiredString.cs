using System;

namespace RequestFiend.UI.Models.Validation;

// For now we don't need composable validation, but if we do, something like this will be the next step:
// https://learn.microsoft.com/en-us/dotnet/architecture/maui/validation

public class RequiredString {
    public static implicit operator string(RequiredString requiredValue) => requiredValue.Value ?? throw new InvalidOperationException();

    private string? value;
    public string errorMessage;

    public string? Value {
        get => value;
        set {
            IsValid = !string.IsNullOrWhiteSpace(value);
            this.value = value;
        }
    }
    public bool IsValid { get; private set; }
    public string? Error => IsValid ? null : errorMessage;

    public RequiredString(string errorMessage) {
        this.errorMessage = errorMessage;
    }
}
