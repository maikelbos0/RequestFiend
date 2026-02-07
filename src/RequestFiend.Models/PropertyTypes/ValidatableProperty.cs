using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace RequestFiend.Models.PropertyTypes;

public class ValidatableProperty<TProperty> : ObservableObject {
    public Func<TProperty> DefaultValueProvider { get; private set; }
    public Func<TProperty, bool> Validator { get; }

    public TProperty Value {
        get => field;
        set {
            SetProperty(ref field, value);
            HasError = !Validator(value);
            IsModified = !HasError && !EqualityComparer<TProperty>.Default.Equals(value, DefaultValueProvider());
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

    public ValidatableProperty(Func<TProperty> defaultValueProvider) : this(defaultValueProvider, _ => true) { }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, Func<TProperty, bool>? validator) {
        DefaultValueProvider = defaultValueProvider;
        Validator = validator ?? (_ => false);
        Value = DefaultValueProvider();
    }

    public void Reset(Func<TProperty> defaultValueProvider) {
        DefaultValueProvider = defaultValueProvider;
        Reset();
    }

    public void Reset() {
        Value = DefaultValueProvider();
    }
}
