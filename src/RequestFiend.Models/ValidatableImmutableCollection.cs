using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RequestFiend.Models;

public class ValidatableImmutableCollection<TImmutable> : ObservableCollection<TImmutable>, IValidatable where TImmutable : IImmutable {
    public bool HasError => false;

    public bool IsModified {
        get;
        private set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsModified)));
            }
        }
    }

    public bool HasItems {
        get;
        private set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasItems)));
            }
        }
    }

    public ValidatableImmutableCollection(IEnumerable<TImmutable> collection) : base(collection) {
        CollectionChanged += OnCollectionChanged;

        HasItems = Count > 0;
        IsModified = false;
    }

    private void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e) {
        HasItems = Count > 0;
        IsModified = true;
    }

    public void Reset() {
        IsModified = false;
    }
}
