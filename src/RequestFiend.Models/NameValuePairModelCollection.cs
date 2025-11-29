using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RequestFiend.Models;

public partial class NameValuePairModelCollection : ObservableCollection<NameValuePairModel> {
    public bool HasItems => Count > 0;
    public bool HasError => this.Any(item => item.HasError);
    
    public NameValuePairModelCollection(List<NameValuePair> collection) {
        CollectionChanged += (sender, e) => OnPropertyChanged(new(nameof(HasItems)));

        foreach (var item in collection) {
            Add(new NameValuePairModel(item));
        }
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
    }
}
