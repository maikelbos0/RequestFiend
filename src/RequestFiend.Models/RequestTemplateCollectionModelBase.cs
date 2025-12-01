using RequestFiend.Core;
using System.IO;

namespace RequestFiend.Models;

public class RequestTemplateCollectionModelBase : BoundModelBase {
    protected readonly string filePath;
    protected readonly RequestTemplateCollection collection;

    public string Title { get; }

    public RequestTemplateCollectionModelBase(string filePath, RequestTemplateCollection collection) {
        this.filePath = filePath;
        this.collection = collection;
        Title = Path.GetFileNameWithoutExtension(filePath);
    }
}
