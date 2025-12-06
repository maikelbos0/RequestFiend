using System;

namespace RequestFiend.Models.Services;

public interface ITransientDataProvider<TData> {
    IDisposable CreateScope(TData data);
    TData GetData();
}