using CommunityToolkit.Mvvm.ComponentModel;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System.ComponentModel;

namespace RequestFiend.Models;

public class NameValuePairModel : ObservableObject {
    public ValidatableString Name { get; set; }
    public ValidatableString Value { get; set; }
    public bool IsModified {
        get => field;
        set => SetProperty(ref field, value);
    }
    public bool HasError {
        get => field;
        set => SetProperty(ref field, value);
    }

    public NameValuePairModel() : this(new(true), new(true)) { }

    public NameValuePairModel(NameValuePair pair) : this(new(true, () => pair.Name), new(true, () => pair.Value)) { }

    private NameValuePairModel(ValidatableString name, ValidatableString value) {
        Name = name;
        Value = value;
        HasError = Name.HasError || Value.HasError;
        IsModified = Name.IsModified || Value.IsModified;

        Name.PropertyChanged += OnPropertyChanged;
        Value.PropertyChanged += OnPropertyChanged;
    }

    public void Reinitialize(NameValuePair pair) {
        Name.Reinitialize(() => pair.Name);
        Value.Reinitialize(() => pair.Value);
    }

    private void OnPropertyChanged(object? _, PropertyChangedEventArgs e) {
        if (e.PropertyName == Constants.IsModified) {
            IsModified = Name.IsModified || Value.IsModified;
        }
        if (e.PropertyName == Constants.HasError) {
            HasError = Name.HasError || Value.HasError;
        }
    }
}
