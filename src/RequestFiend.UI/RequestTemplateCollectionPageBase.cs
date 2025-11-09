using RequestFiend.Core;

namespace RequestFiend.UI;

public class RequestTemplateCollectionPageBase<TModel> : ContentPage<TModel> where TModel : class {
    protected readonly string filePath;
    protected readonly RequestTemplateCollection collection;

    public RequestTemplateCollectionPageBase(string filePath, RequestTemplateCollection collection) {
        this.filePath = filePath;
        this.collection = collection;
    }
}
