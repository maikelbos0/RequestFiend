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
    private bool isCollectionModified = false;

    public bool HasItems {
        get => field;
        set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasItems)));
            }
        }
    }
    public bool IsModified {
        get => field;
        set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsModified)));
            }
        }
    }
    public bool HasError {
        get => field;
        set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasError)));
            }
        }
    }

    public NameValuePairModelCollection(List<NameValuePair> collection) {
        CollectionChanged += OnCollectionChanged;

        foreach (var item in collection) {
            Add(new NameValuePairModel(item));
        }

        IsModified = isCollectionModified = false;
    }

    [RelayCommand]
    public void OnRemoveClicked(NameValuePairModel pair) {
        Remove(pair);
    }

    [RelayCommand]
    public void OnAddClicked() {
        Add(new());
    }

    public void Reinitialize(List<NameValuePair> collection) {
        if (collection.Count != Count) {
            throw new ArgumentException("Collection must have identical length", nameof(collection));
        }

        for (var i = 0; i < collection.Count; i++) {
            this[i].Reinitialize(collection[i]);
        }

        IsModified = isCollectionModified = false;
    }

    private void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e) {
        HasItems = Count > 0;
        HasError = this.Any(item => item.HasError);
        IsModified = isCollectionModified = true;

        if (e.OldItems != null) {
            foreach (var item in e.OldItems) {
                ((NameValuePairModel)item).PropertyChanged -= OnItemPropertyChanged;
            }
        }

        if (e.NewItems != null) {
            foreach (var item in e.NewItems) {
                ((NameValuePairModel)item).PropertyChanged += OnItemPropertyChanged;
            }
        }
    }

    private void OnItemPropertyChanged(object? _, PropertyChangedEventArgs e) {
        if (e.PropertyName == Constants.HasError) {
            HasError = this.Any(item => item.HasError);
        }

        if (e.PropertyName == Constants.IsModified) {
            IsModified = isCollectionModified || this.Any(item => item.IsModified);
        }
    }
}
