using System;

namespace RequestFiend.Models.Services;

public interface IModelDataProvider<TData> where TData : class {
    IDisposable CreateScope(TData data);
    TData GetData();
}