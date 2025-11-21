using System.Collections.ObjectModel;

namespace RequestFiend.Models;

public class NameValuePairModelCollection : ObservableCollection<NameValuePairModel> {
    public bool HasItems => Count > 0;

    public NameValuePairModelCollection() {
        CollectionChanged += (sender, e) => OnPropertyChanged(new(nameof(HasItems)));
    }
}
