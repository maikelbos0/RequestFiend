using System.ComponentModel;

namespace RequestFiend.Models;

public interface IValidatable : INotifyPropertyChanged {
    bool HasError { get; }
    bool IsModified { get; }
}