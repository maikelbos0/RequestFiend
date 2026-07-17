using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RequestFiend.Models;

public class ValidatableImmutableCollection<TImmutable> : ObservableCollection<TImmutable>, IValidatable where TImmutable : IImmutable {
    private readonly Func<IEnumerable<TImmutable>> getter;
    private readonly Action<IEnumerable<TImmutable>> setter;

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

    public ValidatableImmutableCollection(Func<IEnumerable<TImmutable>> getter, Action<IEnumerable<TImmutable>> setter) : base(getter()) {
        this.getter = getter;
        this.setter = setter;

        CollectionChanged += OnCollectionChanged;

        HasItems = Count > 0;
        IsModified = false;
    }

    private void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e) {
        HasItems = Count > 0;
        IsModified = true;
    }

    public void Set() {
        setter(this);
        IsModified = false;
    }

    public void Reset() {
        Clear();
        foreach (var item in getter()) {
            Add(item);
        }
        IsModified = false;
    }
}
