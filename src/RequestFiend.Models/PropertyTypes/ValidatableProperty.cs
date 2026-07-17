using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RequestFiend.Models.PropertyTypes;

public abstract partial class ValidatableProperty : ObservableObject, IValidatable {
    [ObservableProperty] public partial bool HasError { get; protected set; }
    [ObservableProperty] public partial bool IsModified { get; protected set; }
    [ObservableProperty] public partial bool IsModifiedWithoutError { get; protected set; }

    public abstract void Set();

    public abstract void Reset();
}

public sealed class ValidatableProperty<TProperty> : ValidatableProperty {
    private readonly Func<TProperty> getter;
    private readonly Func<TProperty, bool> validator;
    private readonly Action<TProperty>? setter;
    private TProperty initialValue;
    private TProperty value;

    public TProperty Value {
        get => value;
        set {
            if (SetProperty(ref this.value, value)) {
                UpdateState();
            }
        }
    }

    public ValidatableProperty(Func<TProperty> getter, Func<TProperty, bool> validator, params IValidatable[] dependencies) : this(getter, null, validator, dependencies) { }

    public ValidatableProperty(Func<TProperty> getter, Action<TProperty>? setter, params IValidatable[] dependencies) : this(getter, setter, _ => true, dependencies) { }

    public ValidatableProperty(Func<TProperty> getter, Action<TProperty>? setter, Func<TProperty, bool> validator, params IValidatable[] dependencies) {
        this.getter = getter;
        this.setter = setter;
        this.validator = validator;
        initialValue = value = getter();

        UpdateState();

        foreach (var dependency in dependencies) {
            dependency.PropertyChanged += OnDependencyChanged;
        }
    }

    public override void Set() {
        setter?.Invoke(value);
        initialValue = value;
        UpdateState();
    }

    public override void Reset() {
        initialValue = value = getter();
        UpdateState();
    }

    private void OnDependencyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IValidatable.IsModified) || e.PropertyName == nameof(IValidatable.HasError)) {
            UpdateState();
        }
    }

    private void UpdateState() {
        HasError = !validator(Value);
        IsModified = !EqualityComparer<TProperty>.Default.Equals(Value, initialValue);
        IsModifiedWithoutError = IsModified && !HasError;
    }
}
