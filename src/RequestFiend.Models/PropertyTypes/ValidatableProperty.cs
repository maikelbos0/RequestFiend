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
    private Func<TProperty> defaultValueProvider;
    private TProperty value;
    private readonly Action<TProperty>? updater;

    public Func<TProperty, bool> Validator { get; }
    public TProperty Value {
        get => value;
        set {
            if (SetProperty(ref this.value, value)) {
                UpdateState();
            }
        }
    }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, params IValidatable[] dependencies) : this(defaultValueProvider, null, _ => true, dependencies) { }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, Func<TProperty, bool> validator, params IValidatable[] dependencies) : this(defaultValueProvider, null, validator, dependencies) { }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, Action<TProperty>? updater, params IValidatable[] dependencies) : this(defaultValueProvider, updater, _ => true, dependencies) { }

    public ValidatableProperty(Func<TProperty> defaultValueProvider, Action<TProperty>? updater, Func<TProperty, bool> validator, params IValidatable[] dependencies) {
        this.defaultValueProvider = defaultValueProvider;
        Validator = validator;
        value = defaultValueProvider();
        UpdateState();
        
        this.updater = updater;

        foreach (var dependency in dependencies) {
            dependency.PropertyChanged += OnDependencyChanged;
        }
    }

    // TODO refactor to get rid of?
    public void Reset(Func<TProperty> defaultValueProvider) {
        this.defaultValueProvider = defaultValueProvider;
        Reset();
    }

    public void Reset() {
        value = defaultValueProvider();
        UpdateState();
    }

    private void OnDependencyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IValidatable.IsModified) || e.PropertyName == nameof(IValidatable.HasError)) {
            UpdateState();
        }
    }

    private void UpdateState() {
        HasError = !Validator(Value);
        IsModified = !EqualityComparer<TProperty>.Default.Equals(Value, defaultValueProvider());
        IsModifiedWithoutError = IsModified && !HasError;
        updater?.Invoke(Value);
    }
}
