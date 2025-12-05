using RequestFiend.Core;
using System;

namespace RequestFiend.Models.Services;

public class RequestTemplateCollectionProvider : IRequestTemplateCollectionProvider {
    private (string FilePath, RequestTemplateCollection Collection)? data;

    private class Scope : IDisposable {
        private readonly RequestTemplateCollectionProvider provider;

        public Scope(RequestTemplateCollectionProvider provider, string filePath, RequestTemplateCollection collection) {
            this.provider = provider;
            provider.data = (filePath, collection);
        }

        public void Dispose() {
            provider.data = null;
        }
    }

    public IDisposable CreateScope(string filePath, RequestTemplateCollection collection) {
        if (data != null) {
            throw new InvalidOperationException("Only one scope at a time is allowed.");
        }

        return new Scope(this, filePath, collection);
    }

    public (string FilePath, RequestTemplateCollection Collection) GetData() {
        if (data == null) {
            throw new InvalidOperationException("A scope is required.");
        }

        return data.Value;
    }
}
