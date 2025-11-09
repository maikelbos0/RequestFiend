using RequestFiend.Core;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPageBase<TModel> : ContentPage<TModel> where TModel : class {
    protected readonly string filePath;
    protected readonly RequestTemplateCollection collection;

    public RequestTemplateCollectionPageBase(string filePath, RequestTemplateCollection collection) {
        this.filePath = filePath;
        this.collection = collection;
    }

    public Task SaveCollection()
        => File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));
}
