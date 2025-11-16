using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models;
using RequestFiend.UI.Messages;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPageBase<TModel> : ContentPage<TModel> where TModel : ModelBase {
    protected readonly string filePath;
    protected readonly RequestTemplateCollection collection;

    public RequestTemplateCollectionPageBase(string filePath, RequestTemplateCollection collection) {
        this.filePath = filePath;
        this.collection = collection;
        Title = Path.GetFileNameWithoutExtension(filePath);
    }

    public async Task SaveCollection() {
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));

        // Send message also without token so globally it can be known that a collection was updated
        WeakReferenceMessenger.Default.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection), filePath);
        WeakReferenceMessenger.Default.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection));
    }
}
