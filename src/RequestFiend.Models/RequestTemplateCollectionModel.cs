using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionModel : BoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IMessageService messageService;
    private readonly string filePath;
    private readonly RequestTemplateCollection collection;

    public string Title { get; }
    public ValidatableString DefaultUrl { get; set; }
    public NameValuePairModelCollection DefaultHeaders { get; set; }
    public NameValuePairModelCollection Variables { get; set; }

    public RequestTemplateCollectionModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IMessageService messageService, 
        IModelDataProvider<(string, RequestTemplateCollection)> requestTemplateProvider
    ) {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.messageService = messageService;
        (filePath, collection) = requestTemplateProvider.GetData();

        Title = Path.GetFileNameWithoutExtension(filePath);
        DefaultUrl = new(false, () => collection.DefaultUrl);
        DefaultHeaders = new(collection.DefaultHeaders);
        Variables = new(collection.Variables);
    }

    [RelayCommand]
    public async Task Update() {
        if (DefaultHeaders.Any(defaultHeader => defaultHeader.Name.HasError || defaultHeader.Value.HasError) || Variables.Any(variable => variable.Name.HasError || variable.Value.HasError)) {
            return;
        }

        collection.DefaultUrl = DefaultUrl.Value;
        collection.DefaultHeaders = [.. DefaultHeaders.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        collection.Variables = [.. Variables.Select(variable => new NameValuePair() { Name = variable.Name.Value!, Value = variable.Value.Value!, })];

        DefaultUrl.Reset();
        DefaultHeaders.Reinitialize(collection.DefaultHeaders);
        Variables.Reinitialize(collection.Variables);

        await requestTemplateCollectionService.Save(filePath, collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
    }
}
