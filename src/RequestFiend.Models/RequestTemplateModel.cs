using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestTemplateModel : BoundModelBase {
    private readonly static JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly RequestTemplateCollectionFileModel file;
    private readonly RequestTemplateCollection collection;
    private readonly RequestTemplate request;

    // TODO cleanup
    public Guid Id => request.Id;

    public ValidatableProperty<string?> Name { get; set; }
    public ValidatableProperty<string?> Method { get; set; }
    public ValidatableProperty<string?> Url { get; set; }
    public NameValuePairModelCollection Headers { get; set; }
    public ValidatableProperty<string?> ContentType { get; set; }
    public bool UsesStringContent { get => field; private set => SetProperty(ref field, value); }
    public bool UsesJsonContent { get => field; private set => SetProperty(ref field, value); }
    public ValidatableProperty<string?> StringContent { get; set; }

    public RequestTemplateModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        RequestTemplateCollectionFileModel file,
        RequestTemplateCollection collection,
        RequestTemplate request
    ) : base($"{file.Name} - {request.Name}", request.Name) {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.popupService = popupService;
        this.messageService = messageService;
        this.file = file;
        this.collection = collection;
        this.request = request;

        Name = new(() => request.Name, Validator.Required);
        Method = new(() => request.Method, Validator.Required);
        Url = new(() => request.Url, Validator.Required);
        Headers = new(request.Headers);
        ContentType = new(() => Options.ContentTypeMap[request.ContentType], Validator.Required);
        StringContent = new(() => request.StringContent);

        ContentType.PropertyChanged += OnContentTypeChanged;
        UsesStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text] || ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
        UsesJsonContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];

        ConfigureState([Name, Method, Url, ContentType, StringContent], [Headers]);
    }

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        request.Name = Name.Value!;
        request.Method = Method.Value!;
        request.Url = Url.Value!;
        request.Headers = [.. Headers.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        request.ContentType = Options.ReverseContentTypeMap[ContentType.Value!];
        request.StringContent = StringContent.Value;

        Name.Reset();
        Method.Reset();
        Url.Reset();
        Headers.Reset(request.Headers);
        ContentType.Reset();
        StringContent.Reset();
        PageTitleBase = $"{file.Name} - {request.Name}";
        ShellItemTitleBase = request.Name;

        await requestTemplateCollectionService.Save(file.FilePath, collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
    }

    [RelayCommand]
    public async Task ValidateJson() {
        try {
            if (!string.IsNullOrEmpty(StringContent.Value)) {
                _ = JsonDocument.Parse(StringContent.Value);
            }
            messageService.Send(new SuccessMessage("JSON content has been validated"));
        }
        catch (Exception exception) {
            await popupService.ShowErrorPopup($"Failed to validate JSON content: {exception.Message}");
        }
    }

    [RelayCommand]
    public async Task FormatJson() {
        try {
            if (!string.IsNullOrEmpty(StringContent.Value)) {
                var document = JsonDocument.Parse(StringContent.Value ?? "");
                StringContent.Value = JsonSerializer.Serialize(document, jsonSerializerOptions);
            }
            messageService.Send(new SuccessMessage("JSON content has been formatted"));
        }
        catch (Exception exception) {
            await popupService.ShowErrorPopup($"Failed to format JSON content: {exception.Message}");
        }
    }

    [RelayCommand]
    public async Task Delete() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to delete this request?")) {
            collection.Requests.Remove(request);
            await requestTemplateCollectionService.Save(file.FilePath, collection);
            messageService.Send(new RequestTemplateDeletedMessage(), request.Id);
            messageService.Send(new SuccessMessage("Request had been deleted"));
        }
    }

    private void OnContentTypeChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == Constants.Value) {
            UsesStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text] || ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
            UsesJsonContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
        }
    }
}
