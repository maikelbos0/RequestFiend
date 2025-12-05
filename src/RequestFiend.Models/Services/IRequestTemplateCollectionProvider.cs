using RequestFiend.Core;
using System;

namespace RequestFiend.Models.Services;

public interface IRequestTemplateCollectionProvider {
    IDisposable CreateScope(string filePath, RequestTemplateCollection collection);
    (string FilePath, RequestTemplateCollection Collection) GetData();
}