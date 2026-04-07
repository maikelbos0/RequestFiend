using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace RequestFiend.Models.PropertyTypes;

public abstract partial class ValidatableProperty : ObservableObject, IValidatable {
    [ObservableProperty] public partial bool HasError { get; set; }
    [ObservableProperty] public partial bool IsModified { get; set; }
    [ObservableProperty] public partial bool IsModifiedWithoutError { get; set; }
}

public sealed class ValidatableProperty<TProperty> : ValidatableProperty {
    public Func<TProperty> DefaultValueProvider { get; private set; }
    public Func<TProperty, bool> Validator { get; }
    public TProperty Value {
        get => field;
        set {
            if (SetProperty(ref field, value)) {
                SetState();
            }
        }
    }

    public ValidatableProperty(Func<TProperty> defaultValueProvider) : this(defaultValueProvider, _ => true) { }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, Func<TProperty, bool> validator) {
        DefaultValueProvider = defaultValueProvider;
        Validator = validator;
        Value = DefaultValueProvider();
        SetState();
    }

    public void Reset(Func<TProperty> defaultValueProvider) {
        DefaultValueProvider = defaultValueProvider;
        Reset();
    }

    public void Reset() {
        Value = DefaultValueProvider();
        SetState();
    }

    private void SetState() {
        HasError = !Validator(Value);
        IsModified = !EqualityComparer<TProperty>.Default.Equals(Value, DefaultValueProvider());
        IsModifiedWithoutError = IsModified && !HasError;
    }
}
