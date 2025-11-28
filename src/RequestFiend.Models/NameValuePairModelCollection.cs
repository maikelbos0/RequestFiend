using RequestFiend.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RequestFiend.Models;

public class NameValuePairModelCollection : ObservableCollection<NameValuePairModel> {
    public bool HasItems => Count > 0;

    [Obsolete]
    public NameValuePairModelCollection() : this([]) { }

    public NameValuePairModelCollection(List<NameValuePair> collection) {
        CollectionChanged += (sender, e) => OnPropertyChanged(new(nameof(HasItems)));

        foreach (var item in collection) {
            Add(new NameValuePairModel(item));
        }
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
