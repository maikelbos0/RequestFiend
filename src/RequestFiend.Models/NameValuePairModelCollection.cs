using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RequestFiend.Models;

public partial class NameValuePairModelCollection : ObservableCollection<NameValuePairModel> {
    private int unmodifiedCount;

    public bool IsModified {
        get => field;
        private set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsModified)));
            }
        }
    }

    public bool HasItems {
        get => field;
        private set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasItems)));
            }
        }
    }

    public NameValuePairModelCollection(List<NameValuePair> collection) {
        CollectionChanged += OnCollectionChanged;

        foreach (var item in collection) {
            Add(new(item));
        }

        unmodifiedCount = Count;
        IsModified = false;
    }

    [RelayCommand]
    public void OnRemoveClicked(NameValuePairModel pair)
        => Remove(pair);

    [RelayCommand]
    public void OnAddClicked()
        => Add(new());

    public void Reset(List<NameValuePair> collection) {
        if (collection.Count != Count) {
            throw new ArgumentException("Collection must have identical length", nameof(collection));
        }

        for (var i = 0; i < collection.Count; i++) {
            this[i].Reset(collection[i]);
        }

        unmodifiedCount = Count;
        IsModified = false;
    }

    private void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e) {
        HasItems = Count > 0;
        IsModified = Count != unmodifiedCount;
    }
}
