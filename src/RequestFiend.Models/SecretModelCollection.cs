using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class SecretModelCollection : ObservableCollection<SecretModel>, IValidatable {
    private readonly ISecretOwner owner;
    private readonly List<Secret> collection;
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

    public SecretModelCollection(ISecretOwner owner, List<Secret> collection) {
        this.owner = owner;
        this.collection = collection;

        CollectionChanged += OnCollectionChanged;

        foreach (var secret in collection) {
            base.Add(new(owner, secret));
        }

        IsModified = false;
    }

    [RelayCommand]
    public new void Remove(SecretModel secret) {
        var index = IndexOf(secret);

        if (index > -1) {
            changes.Enqueue(() => collection.RemoveAt(index));
            base.Remove(secret);
        }
    }

    [RelayCommand]
    public void Add()
        => Add(new Secret() { Name = "" });

    public void Add(Secret secret) {
        base.Add(new(owner, secret));
        changes.Enqueue(() => collection.Add(secret));
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
        foreach (var secret in collection) {
            Add(secret);
        }
        IsModified = false;
    }

    private void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e) {
        if (e.OldItems != null) {
            foreach (var secret in e.OldItems.Cast<SecretModel>()) {
                secret.PropertyChanged -= OnValidatableChanged;
            }
        }

        if (e.NewItems != null) {
            foreach (var secret in e.NewItems.Cast<SecretModel>()) {
                secret.PropertyChanged += OnValidatableChanged;
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
}
