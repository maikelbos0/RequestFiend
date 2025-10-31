using Microsoft.Maui.Controls;
using RequestFiend.Core;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : ContentPage {
    public RequestTemplateCollectionPage(string name, string filePath) : this(new RequestTemplateCollection() { Name = name }, filePath) { }

    public RequestTemplateCollectionPage(RequestTemplateCollection requestTemplateCollection, string filePath) {
        InitializeComponent();

        RequestTemplateCollection = requestTemplateCollection;
        FilePath = filePath;
        Title = requestTemplateCollection.Name;
    }

    public RequestTemplateCollection RequestTemplateCollection { get; }
    public string FilePath { get; }
}
