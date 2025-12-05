using RequestFiend.Core;
using System;

namespace RequestFiend.Models.Services;

public class RequestTemplateCollectionProvider {
    private class Scope : IDisposable {
        private readonly RequestTemplateCollectionProvider provider;

        public Scope(RequestTemplateCollectionProvider provider, string filePath, RequestTemplateCollection collection) {
            this.provider = provider;
            provider.Data = (filePath, collection);
        }

        public void Dispose() {
            provider.Data = null;
        }
    }

    public (string FilePath, RequestTemplateCollection Collection)? Data { get; private set; }

    public IDisposable CreateScope(string filePath, RequestTemplateCollection collection) {
        if (Data != null) {
            throw new InvalidOperationException("Only one scope at a time is allowed; ensure previous scopes are disposed of");
        }

        return new Scope(this, filePath, collection);
    }
}
