using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class BoundModelBase : ObservableObject, IValidatable {
    private List<IValidatable> validatables = [];

    [ObservableProperty] public partial bool HasError { get; private set; }
    [ObservableProperty] public partial bool IsModified { get; private set; }
    [ObservableProperty] public partial bool IsModifiedWithoutError { get; private set; }
    public IEnumerable<IValidatable> Validatables => validatables;

    public void ConfigureState(IEnumerable<IValidatable> validatables) {
        foreach (var addedValidatable in validatables.Except(this.validatables)) {
            addedValidatable.PropertyChanged += OnValidatableChanged;
        }

        foreach (var removedValidatable in this.validatables.Except(validatables)) {
            removedValidatable.PropertyChanged -= OnValidatableChanged;
        }

        this.validatables = [.. validatables];

        UpdateState();
    }

    private void OnValidatableChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IValidatable.IsModified) || e.PropertyName == nameof(IValidatable.HasError)) {
            UpdateState();
        }
    }

    protected virtual void UpdateState() {
        HasError = validatables.Any(validatableProperty => validatableProperty.HasError);
        IsModified = validatables.Any(validatableProperty => validatableProperty.IsModified);
        IsModifiedWithoutError = IsModified && !HasError;
    }

    public virtual void Set() {
        foreach (var validatable in validatables) {
            validatable.Set();
        }
    }

    public void Reset() {
        foreach (var validatable in validatables) {
            validatable.Reset();
        }
    }
}
