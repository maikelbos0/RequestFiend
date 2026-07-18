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
    private readonly List<NameValuePair> collection;
    private readonly Func<string, bool> nameValidator;
    private readonly Func<string, bool> valueValidator;
    private readonly IValidatable[] dependencies;
    private readonly Queue<Action> changes = [];

    public bool HasError {
        get;
        private set {
            if (field != value) {
                field = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasError)));
            }
        }
    }

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

    public NameValuePairModelCollection(List<NameValuePair> collection, Func<string, bool> nameValidator, params IValidatable[] dependencies)
        : this(collection, nameValidator, _ => true, dependencies) { }

    public NameValuePairModelCollection(List<NameValuePair> collection, Func<string, bool> nameValidator, Func<string, bool> valueValidator, params IValidatable[] dependencies) {
        this.collection = collection;
        this.nameValidator = nameValidator;
        this.valueValidator = valueValidator;
        this.dependencies = dependencies;

        CollectionChanged += OnCollectionChanged;

        foreach (var pair in collection) {
            base.Add(new(pair, nameValidator, valueValidator, dependencies));
        }

        IsModified = false;
    }

    [RelayCommand]
    public new void Remove(NameValuePairModel pair) {
        var index = IndexOf(pair);

        if (index > -1) {
            changes.Enqueue(() => collection.RemoveAt(index));
            base.Remove(pair);
        }
    }

    [RelayCommand]
    public void Add()
        => Add(new NameValuePair() { Name = "", Value = "" });

    public void Add(string name, string value)
        => Add(new NameValuePair() { Name = name, Value = value });

    public void Add(NameValuePair pair) {
        base.Add(new(pair, nameValidator, valueValidator, dependencies));
        changes.Enqueue(() => collection.Add(pair));
    }

    public void Set() {
        while (changes.TryDequeue(out var change)) {
            change();
        }

        foreach (var pair in this) {
            pair.Set();
        }

        IsModified = false;
    }

    public void Reset() {
        Clear();
        foreach (var pair in collection) {
            Add(pair);
        }
        IsModified = false;
    }

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
        IsModified = Count != collection.Count || this.Any(nameValuePairModel => nameValuePairModel.IsModified);
    }

    public List<NameValuePair> CreateNameValuePairs()
        => [.. this.Select(pair => pair.CreateNameValuePair())];
}
