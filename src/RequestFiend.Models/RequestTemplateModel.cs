using CommunityToolkit.Mvvm.ComponentModel;
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

public partial class RequestTemplateModel : PageBoundModelBase {
    private readonly static JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;

    public RequestTemplateCollectionFileModel File { get; }
    public RequestTemplateCollection Collection { get; }
    public RequestTemplate Request { get; }

    public string Id { get; } = Guid.NewGuid().ToString();
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Method { get; }
    public ValidatableProperty<string> Url { get; }
    public NameValuePairModelCollection Headers { get; }
    public ValidatableProperty<string> ContentType { get; }
    [ObservableProperty] public partial bool UsesStringContent { get; private set; }
    [ObservableProperty] public partial bool UsesStructuredStringContent { get; private set; }
    [ObservableProperty] public partial bool UsesUnstructuredStringContent { get; private set; }
    public ValidatableProperty<string> StringContent { get; }
    public ValidatableProperty<string> PreExchangeScript { get; }
    public ValidatableProperty<string> PostExchangeScript { get; }
    public ValidatableProperty<string> OnExceptionScript { get; }

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

        File = file;
        Collection = collection;
        Request = request;

        Name = new(() => request.Name, Validator.Required);
        Method = new(() => request.Method, Validator.Required);
        Url = new(() => request.Url, Validator.Required);
        Headers = new(request.Headers, Validator.Required);
        ContentType = new(() => Options.ContentTypeMap[request.ContentType], Validator.Required);
        StringContent = new(() => request.StringContent);
        PreExchangeScript = new(() => request.PreExchangeScript);
        PostExchangeScript = new(() => request.PostExchangeScript);
        OnExceptionScript = new(() => request.OnExceptionScript);

        ContentType.PropertyChanged += OnContentTypeChanged;
        UsesStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text] || ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
        UsesStructuredStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
        UsesUnstructuredStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text];

        ConfigureState([Name, Method, Url, Headers, ContentType, StringContent, PreExchangeScript, PostExchangeScript, OnExceptionScript]);
    }

    [RelayCommand]
    public void CreateRequest() {
        if (HasError) {
            return;
        }

        var request = Request.Clone();

        request.Name = Name.Value!;
        request.Method = Method.Value!;
        request.Url = Url.Value!;
        request.Headers = [.. Headers.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        request.ContentType = Options.ReverseContentTypeMap[ContentType.Value!];
        request.StringContent = StringContent.Value;
        request.PreExchangeScript = PreExchangeScript.Value;
        request.PostExchangeScript = PostExchangeScript.Value;
        request.OnExceptionScript = OnExceptionScript.Value;

        messageService.Send(new CreateRequestMessage(File.FilePath, Id, Collection, request));
    }

    [RelayCommand]
    public async Task Update() {
        if (HasError) {
            return;
        }

        Request.Name = Name.Value!;
        Request.Method = Method.Value!;
        Request.Url = Url.Value!;
        Request.Headers = [.. Headers.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        Request.ContentType = Options.ReverseContentTypeMap[ContentType.Value!];
        Request.StringContent = StringContent.Value;
        Request.PreExchangeScript = PreExchangeScript.Value;
        Request.PostExchangeScript = PostExchangeScript.Value;
        Request.OnExceptionScript = OnExceptionScript.Value;

        Name.Reset();
        Method.Reset();
        Url.Reset();
        Headers.Reset(Request.Headers);
        ContentType.Reset();
        StringContent.Reset();
        PreExchangeScript.Reset();
        PostExchangeScript.Reset();
        OnExceptionScript.Reset();

        PageTitleBase = $"{File.Name} - {Request.Name}";
        ShellItemTitleBase = Request.Name;

        await requestTemplateCollectionService.Save(File.FilePath, Collection);
        messageService.Send(new SuccessMessage("Changes have been saved"));
    }

    [RelayCommand]
    public async Task ShowUrlPopup() {
        var result = await popupService.ShowUrlPopup(Url.Value);

        if (result.Result != null) {
            Url.Value = result.Result;
        }
    }

    [RelayCommand]
    public async Task ValidateStructuredText() {
        try {
            _ = JsonDocument.Parse(StringContent.Value);
            messageService.Send(new SuccessMessage("JSON content has been validated"));
        }
        catch (Exception exception) {
            await popupService.ShowErrorPopup($"Failed to validate JSON content: {exception.Message}");
        }
    }

    [RelayCommand]
    public async Task FormatStructuredText() {
        try {
            var document = JsonDocument.Parse(StringContent.Value ?? "");
            StringContent.Value = JsonSerializer.Serialize(document, jsonSerializerOptions);
            messageService.Send(new SuccessMessage("JSON content has been formatted"));
        }
        catch (Exception exception) {
            await popupService.ShowErrorPopup($"Failed to format JSON content: {exception.Message}");
        }
    }

    [RelayCommand]
    public async Task Delete() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to delete this request?")) {
            Collection.Requests.Remove(Request);
            await requestTemplateCollectionService.Save(File.FilePath, Collection);
            messageService.Send(new RequestTemplateDeletedMessage(), Id);
            messageService.Send(new SuccessMessage("Request has been deleted"));
        }
    }

    private void OnContentTypeChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ValidatableProperty<>.Value)) {
            UsesStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text] || ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
            UsesStructuredStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
            UsesUnstructuredStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text];
        }
    }
}
