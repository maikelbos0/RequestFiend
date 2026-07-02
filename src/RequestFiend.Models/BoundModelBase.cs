using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class BoundModelBase : ObservableObject, IValidatable {
    private readonly List<IValidatable> validatables = [];

    [ObservableProperty] public partial bool HasError { get; private set; }
    [ObservableProperty] public partial bool IsModified { get; private set; }
    [ObservableProperty] public partial bool IsModifiedWithoutError { get; private set; }
    public IEnumerable<IValidatable> Validatables => validatables;

    public virtual void ConfigureState(IEnumerable<IValidatable> validatables) {
        this.validatables.AddRange(validatables);

        foreach (var validatable in validatables) {
            validatable.PropertyChanged += OnValidatableChanged;
        }

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
}
