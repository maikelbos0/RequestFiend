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
    private Func<TProperty> getter;
    private readonly Action<TProperty>? setter;
    private TProperty initialValue;
    private TProperty value;

    public Func<TProperty, bool> Validator { get; }
    public TProperty Value {
        get => value;
        set {
            if (SetProperty(ref this.value, value)) {
                UpdateState();
            }
        }
    }

    [Obsolete("See if updater can be made mandatory")]
    public ValidatableProperty(Func<TProperty> getter, params IValidatable[] dependencies) : this(getter, null, _ => true, dependencies) { }

    [Obsolete("See if updater can be made mandatory")]
    public ValidatableProperty(Func<TProperty> getter, Func<TProperty, bool> validator, params IValidatable[] dependencies) : this(getter, null, validator, dependencies) { }

    public ValidatableProperty(Func<TProperty> getter, Action<TProperty>? setter, params IValidatable[] dependencies) : this(getter, setter, _ => true, dependencies) { }

    public ValidatableProperty(Func<TProperty> getter, Action<TProperty>? setter, Func<TProperty, bool> validator, params IValidatable[] dependencies) {
        this.getter = getter;
        Validator = validator;
        initialValue = value = getter();
        UpdateState();
        
        this.setter = setter;

        foreach (var dependency in dependencies) {
            dependency.PropertyChanged += OnDependencyChanged;
        }
    }

    public void Reset() {
        initialValue = value = getter();
        UpdateState();
    }

    private void OnDependencyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IValidatable.IsModified) || e.PropertyName == nameof(IValidatable.HasError)) {
            UpdateState();
        }
    }

    private void UpdateState() {
        HasError = !Validator(Value);
        IsModified = !EqualityComparer<TProperty>.Default.Equals(Value, initialValue);
        IsModifiedWithoutError = IsModified && !HasError;
        setter?.Invoke(Value);
    }
}
