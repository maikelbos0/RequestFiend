using System;

namespace RequestFiend.Models.Services;

public interface IModelDataProvider<TData> where TData : struct {
    IDisposable CreateScope(TData data);
    TData GetData();
}