using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestTemplateCollectionSettingsModel : BoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IMessageService messageService;
    private readonly string filePath;
    private readonly RequestTemplateCollection collection;

    public string PageTitle { get => field; set => SetProperty(ref field, value); }
    public string ShellItemTitle { get => field; set => SetProperty(ref field, value); }
    public ValidatableProperty<string?> DefaultUrl { get; set; }
    public NameValuePairModelCollection DefaultHeaders { get; set; }
    public NameValuePairModelCollection Variables { get; set; }

    public RequestTemplateCollectionSettingsModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IMessageService messageService, 
        IModelDataProvider<(string, RequestTemplateCollection)> modelDataProvider
    ) {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.messageService = messageService;
        (filePath, collection) = modelDataProvider.GetData();

        DefaultUrl = new(() => collection.DefaultUrl);
        DefaultHeaders = new(collection.DefaultHeaders);
        Variables = new(collection.Variables);

        ConfigureState([DefaultUrl], [DefaultHeaders, Variables]);
        UpdateTitles();
        PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(IsModified) || e.PropertyName == nameof(HasError)) {
                UpdateTitles();
            }
        };
    }

    [MemberNotNull(nameof(PageTitle), nameof(ShellItemTitle))]
    public void UpdateTitles() {
        var suffix = HasError ? " ▲" : IsModified ? " ●" : "";

        PageTitle = $"{Path.GetFileNameWithoutExtension(filePath)} - Collection settings{suffix}";
        ShellItemTitle = $"Collection settings{suffix}";
    }

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        collection.DefaultUrl = DefaultUrl.Value;
        collection.DefaultHeaders = [.. DefaultHeaders.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        collection.Variables = [.. Variables.Select(variable => new NameValuePair() { Name = variable.Name.Value!, Value = variable.Value.Value!, })];

        DefaultUrl.Reset();
        DefaultHeaders.Reset(collection.DefaultHeaders);
        Variables.Reset(collection.Variables);

        await requestTemplateCollectionService.Save(filePath, collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
    }
}
