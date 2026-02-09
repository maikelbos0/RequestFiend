using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class NewRequestTemplateModel : BoundModelBase {
    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IMessageService messageService;
    private readonly string filePath;
    private readonly RequestTemplateCollection collection;

    public string PageTitle { get => field; set => SetProperty(ref field, value); }
    public string ShellItemTitle { get => field; set => SetProperty(ref field, value); }
    public ValidatableProperty<string?> Name { get; set; } = new(() => null, Validator.Required);
    public ValidatableProperty<string?> Method { get; set; } = new(() => null, Validator.Required);
    public ValidatableProperty<string?> Url { get; set; }

    public NewRequestTemplateModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IMessageService messageService,
        IModelDataProvider<(string, RequestTemplateCollection)> modelDataProvider
    ) {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.messageService = messageService;
        (filePath, collection) = modelDataProvider.GetData();

        Url = new(() => collection.DefaultUrl, Validator.Required);
        messageService.Register<NewRequestTemplateModel, RequestTemplateCollectionUpdatedMessage, string>(this, filePath, (model, _) => {
            if (!model.Url.IsModified) {
                model.Url.Reset();
            }
        });

        UpdateTitles();
        ConfigureState([Name, Method, Url], []);
        PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(IsModified) || e.PropertyName == nameof(HasError)) {
                UpdateTitles();
            }
        };
    }

    [MemberNotNull(nameof(PageTitle), nameof(ShellItemTitle))]
    public void UpdateTitles() {
        var suffix = HasError ? " ▲" : IsModified ? " ●" : "";

        PageTitle = $"{Path.GetFileNameWithoutExtension(filePath)} - New request{suffix}";
        ShellItemTitle = $"New request{suffix}";
    }

    [RelayCommand]
    public async Task Create() {
        if (HasError) {
            return;
        }

        var request = new RequestTemplate() {
            Name = Name.Value!,
            Method = Method.Value!,
            Url = Url.Value!
        };
        collection.Requests.Add(request);

        Name.Reset();
        Method.Reset();
        Url.Reset();

        await requestTemplateCollectionService.Save(filePath, collection);
        messageService.Send(new OpenTemplateRequestMessage(filePath, collection, request));
        messageService.Send(new SuccessMessage("Request had been added"));
    }
}
