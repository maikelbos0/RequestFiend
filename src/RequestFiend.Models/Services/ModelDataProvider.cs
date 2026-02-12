using System;

namespace RequestFiend.Models.Services;

public class ModelDataProvider<TData> : IModelDataProvider<TData> where TData : class {
    private TData? data;

    private class Scope : IDisposable {
        private readonly ModelDataProvider<TData> provider;

        public Scope(ModelDataProvider<TData> provider, TData data) {
            this.provider = provider;
            provider.data = data;
        }

        public void Dispose() {
            provider.data = default;
        }
    }

    public IDisposable CreateScope(TData data) {
        if (this.data != null) {
            throw new InvalidOperationException("Only one scope at a time is allowed.");
        }

        return new Scope(this, data);
    }

    public TData GetData() {
        if (data == null) {
            throw new InvalidOperationException("A scope is required.");
        }

        return data;
    }
}
