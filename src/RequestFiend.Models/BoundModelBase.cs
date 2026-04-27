using CommunityToolkit.Mvvm.ComponentModel;
using RequestFiend.Models.PropertyTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class BoundModelBase : ObservableObject {
    private List<IValidatable> validatables = [];

    [ObservableProperty] public partial bool HasError { get; set; }
    [ObservableProperty] public partial bool IsModified { get; set; }
    [ObservableProperty] public partial bool IsModifiedWithoutError { get; set; }
    // TODO test results
    public IEnumerable<IValidatable> Validatables => validatables;

    // TODO rename?
    public virtual void ConfigureState(IEnumerable<IValidatable> validatables) {
        this.validatables.AddRange(validatables);

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
