using System;

namespace RequestFiend.Models.Services;

public interface IModelDataProvider {
    IDisposable CreateScope(params object[] data);
    TData GetData<TData>() where TData : class;
}