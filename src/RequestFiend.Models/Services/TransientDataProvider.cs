using System;
using System.Collections.Generic;

namespace RequestFiend.Models.Services;

public class TransientDataProvider<TData> : ITransientDataProvider<TData> {
    private TData? data;

    private class Scope : IDisposable {
        private readonly TransientDataProvider<TData> provider;

        public Scope(TransientDataProvider<TData> provider, TData data) {
            this.provider = provider;
            provider.data = data;
        }

        public void Dispose() {
            provider.data = default;
        }
    }

    public IDisposable CreateScope(TData data) {
        if (!EqualityComparer<TData>.Default.Equals(this.data, default)) {
            throw new InvalidOperationException("Only one scope at a time is allowed.");
        }

        return new Scope(this, data);
    }

    public TData GetData() {
        if (EqualityComparer<TData>.Default.Equals(this.data, default)) {
            throw new InvalidOperationException("A scope is required.");
        }

        return data!;
    }
}
