using CommunityToolkit.Mvvm.ComponentModel;
using RequestFiend.Models.PropertyTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public class BoundModelBase : ObservableObject {

    private IEnumerable<IValidatable> validatables = [];
    public bool HasError { get => field; set => SetProperty(ref field, value); }
    public bool IsModified { get => field; set => SetProperty(ref field, value); }
    public bool IsModifiedWithoutError { get => field; set => SetProperty(ref field, value); }

    public virtual void ConfigureState(IEnumerable<IValidatable> validatables) {
        this.validatables = validatables;

        foreach (var validatableProperty in validatables) {
            validatableProperty.PropertyChanged += OnValidatablePropertyChanged;
        }

        UpdateState();
    }

    private void OnValidatablePropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ValidatableProperty.IsModified) || e.PropertyName == nameof(ValidatableProperty.HasError)) {
            UpdateState();
        }
    }

    protected virtual void UpdateState() {
        HasError = validatables.Any(validatableProperty => validatableProperty.HasError);
        IsModified = validatables.Any(validatableProperty => validatableProperty.IsModified);
        IsModifiedWithoutError = IsModified && !HasError;
    }
}
