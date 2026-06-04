using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
                UpdateState();
            }
        }
    }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, params IValidatable[] dependencies) : this(defaultValueProvider, _ => true, dependencies) { }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, Func<TProperty, bool> validator, params IValidatable[] dependencies) {
        DefaultValueProvider = defaultValueProvider;
        Validator = validator;
        Value = DefaultValueProvider();
        UpdateState();

        foreach (var dependency in dependencies) {
            dependency.PropertyChanged += OnDependencyChanged;
        }
    }

    public void Reset(Func<TProperty> defaultValueProvider) {
        DefaultValueProvider = defaultValueProvider;
        Reset();
    }

    public void Reset() {
        Value = DefaultValueProvider();
        UpdateState();
    }

    private void OnDependencyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IValidatable.IsModified) || e.PropertyName == nameof(IValidatable.HasError)) {
            UpdateState();
        }
    }

    private void UpdateState() {
        HasError = !Validator(Value);
        IsModified = !EqualityComparer<TProperty>.Default.Equals(Value, DefaultValueProvider());
        IsModifiedWithoutError = IsModified && !HasError;
    }
}
