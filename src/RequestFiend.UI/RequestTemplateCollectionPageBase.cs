using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.UI.Messages;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPageBase<TModel> : ContentPage<TModel>, IRecipient<RequestTemplateCollectionUpdatedMessage> where TModel : class {
    protected readonly string filePath;
    protected readonly RequestTemplateCollection collection;

    public RequestTemplateCollectionPageBase(string filePath, RequestTemplateCollection collection) {
        this.filePath = filePath;
        this.collection = collection;
        WeakReferenceMessenger.Default.Register(this, filePath);
        Title = collection.Name;
    }

    public void Receive(RequestTemplateCollectionUpdatedMessage message) {
        Title = message.Collection.Name;
    }

    public async Task SaveCollection() {
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(collection));

        // Send message also without token so globally it can be known that a collection was updated
        WeakReferenceMessenger.Default.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection), filePath);
        WeakReferenceMessenger.Default.Send(new RequestTemplateCollectionUpdatedMessage(filePath, collection));
    }
}
