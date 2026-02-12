using System;
using System.Collections.Generic;

namespace RequestFiend.Models.Services;

public class ModelDataProvider : IModelDataProvider {
    private Dictionary<Type, object> data = [];

    private class Scope : IDisposable {
        private readonly ModelDataProvider provider;
        private readonly object[] data;

        public Scope(ModelDataProvider provider, object[] data) {
            this.provider = provider;
            this.data = data;

            foreach (var dataItem in data) {
                provider.data.Add(dataItem.GetType(), dataItem);
            }
        }

        public void Dispose() {
            foreach (var dataItem in data) {
                provider.data.Remove(dataItem.GetType());
            }
        }
    }

    public IDisposable CreateScope(params object[] data) {
        return new Scope(this, data);
    }

    public TData GetData<TData>() where TData : class
        => (TData)data[typeof(TData)];
}
