using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class NameValuePairModelCollection : ObservableCollection<NameValuePairModel> {
    private int unmodifiedCount;

    public bool HasError {
        get => field;
        private set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasError)));
            }
        }
    }

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
        if (e.OldItems != null) {
            foreach (var pair in e.OldItems.Cast<NameValuePairModel>()) {
                pair.PropertyChanged -= OnValidatablePropertyChanged;
            }
        }

        if (e.NewItems != null) {
            foreach (var pair in e.NewItems.Cast<NameValuePairModel>()) {
                pair.PropertyChanged += OnValidatablePropertyChanged;
            }
        }

        UpdateState();
    }

    private void OnValidatablePropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(NameValuePairModel.IsModified) || e.PropertyName == nameof(NameValuePairModel.HasError)) {
            UpdateState();
        }
    }

    private void UpdateState() {
        HasItems = Count > 0;
        HasError = this.Any(nameValuePairModel => nameValuePairModel.HasError);
        IsModified = Count != unmodifiedCount || this.Any(nameValuePairModel => nameValuePairModel.IsModified);
    }
}
