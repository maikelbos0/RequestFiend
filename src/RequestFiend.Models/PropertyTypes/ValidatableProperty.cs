using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace RequestFiend.Models.PropertyTypes;

public abstract class ValidatableProperty : ObservableObject {
    public abstract bool HasError { get; protected set; }
    public abstract bool IsModified { get; protected set; }
    public abstract bool IsModifiedWithoutError { get; protected set; }
}

public sealed class ValidatableProperty<TProperty> : ValidatableProperty {
    public Func<TProperty> DefaultValueProvider { get; private set; }
    public Func<TProperty, bool> Validator { get; }

    public TProperty Value {
        get => field;
        set {
            SetProperty(ref field, value);

            // TODO can we call this only when SetProperty returns true? There was an issue but I forgot.

            HasError = !Validator(value);
            IsModified = !EqualityComparer<TProperty>.Default.Equals(value, DefaultValueProvider());
            IsModifiedWithoutError = IsModified && !HasError;
        }
    }
    public override bool HasError { get => field; protected set => SetProperty(ref field, value); }
    public override bool IsModified { get => field; protected set => SetProperty(ref field, value); }
    public override bool IsModifiedWithoutError { get => field; protected set => SetProperty(ref field, value); }

    public ValidatableProperty(Func<TProperty> defaultValueProvider) : this(defaultValueProvider, _ => true) { }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, Func<TProperty, bool> validator) {
        DefaultValueProvider = defaultValueProvider;
        Validator = validator;
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
