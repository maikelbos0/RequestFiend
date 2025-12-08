using System;

namespace RequestFiend.Models.Services;

public interface IModelDataProvider<TData> {
    IDisposable CreateScope(TData data);
    TData GetData();
}