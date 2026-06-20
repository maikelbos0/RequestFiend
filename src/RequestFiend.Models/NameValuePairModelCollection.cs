using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class NameValuePairModelCollection : ObservableCollection<NameValuePairModel>, IValidatable {
    private readonly Func<string, bool> nameValidator;
    private readonly Func<string, bool> valueValidator;
    private readonly IValidatable[] dependencies;
    private int unmodifiedCount;

    public bool HasError {
        get;
        set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasError)));
            }
        }
    }

    public bool IsModified {
        get;
        set {
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

    public NameValuePairModelCollection(List<NameValuePair> collection, Func<string, bool> nameValidator, params IValidatable[] dependencies) 
        : this(collection, nameValidator, _ => true, dependencies) { }

    public NameValuePairModelCollection(List<NameValuePair> collection, Func<string, bool> nameValidator, Func<string, bool> valueValidator, params IValidatable[] dependencies) {
        CollectionChanged += OnCollectionChanged;

        foreach (var item in collection) {
            Add(new(() => item.Name, () => item.Value, nameValidator, valueValidator, dependencies));
        }

        unmodifiedCount = Count;
        IsModified = false;
        this.nameValidator = nameValidator;
        this.valueValidator = valueValidator;
        this.dependencies = dependencies;
    }

    [RelayCommand]
    public new void Remove(NameValuePairModel pair)
        => base.Remove(pair);

    [RelayCommand]
    public void Add()
        => Add("", "");

    public void Add(string name, string value) {
        Add(new(() => name, () => value, nameValidator, valueValidator, dependencies));
    }

    public void Reset(List<NameValuePair> collection) {
        if (collection.Count != Count) {
            throw new ArgumentException("Collection must have identical length.", nameof(collection));
        }

        for (var i = 0; i < collection.Count; i++) {
            this[i].Reset(collection[i]);
        }

        unmodifiedCount = Count;
        IsModified = false;
    }

    public List<NameValuePair> GetNameValuePairs()
        => [.. this.Select(pair => pair.GetNameValuePair())];

    private void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e) {
        if (e.OldItems != null) {
            foreach (var pair in e.OldItems.Cast<NameValuePairModel>()) {
                pair.PropertyChanged -= OnValidatableChanged;
            }
        }

        if (e.NewItems != null) {
            foreach (var pair in e.NewItems.Cast<NameValuePairModel>()) {
                pair.PropertyChanged += OnValidatableChanged;
            }
        }

        UpdateState();
    }

    private void OnValidatableChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IValidatable.IsModified) || e.PropertyName == nameof(IValidatable.HasError)) {
            UpdateState();
        }
    }

    private void UpdateState() {
        HasItems = Count > 0;
        HasError = this.Any(nameValuePairModel => nameValuePairModel.HasError);
        IsModified = Count != unmodifiedCount || this.Any(nameValuePairModel => nameValuePairModel.IsModified);
    }
}
