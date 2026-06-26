using RequestFiend.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RequestFiend.Models;

public class FileModelCollection : ObservableCollection<FileModel>, IValidatable {
    public bool HasError => false;

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

    public FileModelCollection(List<FileModel> collection) : base(collection) {
        CollectionChanged += OnCollectionChanged;

        HasItems = Count > 0;
        IsModified = false;
    }

    private void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs e) {
        HasItems = Count > 0;
        IsModified = true;
    }
}
