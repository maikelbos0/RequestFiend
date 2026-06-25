using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RequestFiend.Models;

public partial class RequestTemplateModel : PageBoundModelBase {
    private readonly static JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;

    public FileModel File { get; }
    public RequestTemplateCollection Collection { get; }
    public RequestTemplate Request { get; }

    public string Id { get; } = Guid.NewGuid().ToString();
    public ValidatableProperty<string> Name { get; }
    public ValidatableProperty<string> Method { get; }
    public ValidatableProperty<string> Url { get; }
    public NameValuePairModelCollection Headers { get; }
    public ValidatableProperty<string> ContentType { get; }
    public ValidatableProperty<bool> HasManualContentTypeHeader { get; }
    [ObservableProperty] public partial bool UsesContent { get; private set; }
    [ObservableProperty] public partial bool UsesStructuredContent { get; private set; }
    [ObservableProperty] public partial bool UsesUnstructuredContent { get; private set; }
    [ObservableProperty] public partial bool UsesStringContent { get; private set; }
    [ObservableProperty] public partial bool UsesFileContent { get; private set; }
    [ObservableProperty] public partial bool UsesFormDataContent { get; private set; }
    public ValidatableProperty<string> StringContent { get; }
    public ValidatableProperty<string> FileContent { get; }
    public NameValuePairModelCollection FormFieldContent { get; }
    public NameValuePairModelCollection FormFileContent { get; }
    public ScriptModel PreExchangeScript { get; }
    public ScriptModel PostExchangeScript { get; }
    public ScriptModel OnExceptionScript { get; }

    public RequestTemplateModel(
        IRequestTemplateCollectionService requestTemplateCollectionService,
        IPopupService popupService,
        IMessageService messageService,
        FileModel file,
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
        HasManualContentTypeHeader = new(() => request.HasManualContentTypeHeader);
        StringContent = new(() => request.StringContent);
        FileContent = new(() => request.FileContent, Validator.Conditional(() => GetContentType() == Core.ContentType.File, Validator.Required), ContentType);
        FormFieldContent = new(request.FormFieldContent, Validator.Conditional(() => GetContentType() == Core.ContentType.FormData, Validator.Required), ContentType);
        FormFileContent = new(request.FormFileContent, Validator.Conditional(() => GetContentType() == Core.ContentType.FormData, Validator.Required), Validator.Conditional(() => GetContentType() == Core.ContentType.FormData, Validator.Required), ContentType);
        PreExchangeScript = new(request.PreExchangeScript);
        PostExchangeScript = new(request.PostExchangeScript);
        OnExceptionScript = new(request.OnExceptionScript);

        ContentType.PropertyChanged += OnContentTypeChanged;
        SetContentTypeUsage();

        ConfigureState([Name, Method, Url, Headers, ContentType, HasManualContentTypeHeader, StringContent, FileContent, FormFieldContent, FormFileContent, PreExchangeScript, PostExchangeScript, OnExceptionScript]);
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
        request.Headers = Headers.GetNameValuePairs();
        request.ContentType = GetContentType();
        request.HasManualContentTypeHeader = HasManualContentTypeHeader.Value;
        request.StringContent = StringContent.Value;
        request.FileContent = FileContent.Value;
        request.FormFieldContent = FormFieldContent.GetNameValuePairs();
        request.FormFileContent = FormFileContent.GetNameValuePairs();
        PreExchangeScript.Update(request.PreExchangeScript);
        PostExchangeScript.Update(request.PostExchangeScript);
        OnExceptionScript.Update(request.OnExceptionScript);

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
        Request.Headers = Headers.GetNameValuePairs();
        Request.ContentType = GetContentType();
        Request.HasManualContentTypeHeader = HasManualContentTypeHeader.Value;
        Request.StringContent = StringContent.Value;
        Request.FileContent = FileContent.Value;
        Request.FormFieldContent = FormFieldContent.GetNameValuePairs();
        Request.FormFileContent = FormFileContent.GetNameValuePairs();
        PreExchangeScript.Update(Request.PreExchangeScript);
        PostExchangeScript.Update(Request.PostExchangeScript);
        OnExceptionScript.Update(Request.OnExceptionScript);

        Name.Reset();
        Method.Reset();
        Url.Reset();
        Headers.Reset(Request.Headers);
        ContentType.Reset();
        HasManualContentTypeHeader.Reset();
        FileContent.Reset();
        StringContent.Reset();
        FormFieldContent.Reset(Request.FormFieldContent);
        FormFileContent.Reset(Request.FormFileContent);
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
        var result = await popupService.ShowUrlPopup(Collection, Url.Value);

        if (result.Result != null) {
            Url.Value = result.Result;
            messageService.Send(new ValidatablePropertyUpdatedMessage(Url));
        }
    }

    [RelayCommand]
    public async Task ValidateStructuredText() {
        if (ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json]) {
            try {
                _ = JsonDocument.Parse(StringContent.Value);
                messageService.Send(new SuccessMessage("JSON content has been validated"));
            }
            catch (Exception exception) {
                await popupService.ShowErrorPopup($"Failed to validate JSON content: {exception.Message}");
            }
        }
        else if (ContentType.Value == Options.ContentTypeMap[Core.ContentType.Xml]) {
            try {
                _ = XDocument.Parse(StringContent.Value ?? "");
                messageService.Send(new SuccessMessage("XML content has been validated"));
            }
            catch (Exception exception) {
                await popupService.ShowErrorPopup($"Failed to validate XML content: {exception.Message}");
            }
        }
        else {
            throw new NotImplementedException($"Missing implementation for {nameof(ContentType)}: {ContentType}");
        }
    }

    [RelayCommand]
    public async Task FormatStructuredText() {
        if (ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json]) {
            try {
                var document = JsonDocument.Parse(StringContent.Value ?? "");
                StringContent.Value = JsonSerializer.Serialize(document, jsonSerializerOptions);
                messageService.Send(new SuccessMessage("JSON content has been formatted"));
            }
            catch (Exception exception) {
                await popupService.ShowErrorPopup($"Failed to format JSON content: {exception.Message}");
            }
        }
        else if (ContentType.Value == Options.ContentTypeMap[Core.ContentType.Xml]) {
            try {
                var document = XDocument.Parse(StringContent.Value ?? "");
                StringContent.Value = document.ToString();
                messageService.Send(new SuccessMessage("XML content has been formatted"));
            }
            catch (Exception exception) {
                await popupService.ShowErrorPopup($"Failed to format XML content: {exception.Message}");
            }
        }
        else {
            throw new NotImplementedException($"Missing implementation for {nameof(ContentType)}: {ContentType}");
        }
    }

    [RelayCommand]
    public async Task PickFileContent() {
        var file = await popupService.ShowPickFileDialog(new());

        if (file != null) {
            FileContent.Value = file.FullPath;
            messageService.Send(new ValidatablePropertyUpdatedMessage(FileContent));
        }
    }

    [RelayCommand]
    public async Task PickFormFileContent(NameValuePairModel pair) {
        var file = await popupService.ShowPickFileDialog(new());

        if (file != null) {
            pair.Value.Value = file.FullPath;
            messageService.Send(new ValidatablePropertyUpdatedMessage(pair.Value));
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
            SetContentTypeUsage();
        }
    }

    private void SetContentTypeUsage() {
        var contentType = GetContentType();

        UsesContent = contentType != Core.ContentType.None;
        UsesStructuredContent = contentType == Core.ContentType.Json || contentType == Core.ContentType.Xml;
        UsesUnstructuredContent = contentType == Core.ContentType.Text || contentType == Core.ContentType.File || contentType == Core.ContentType.FormData;
        UsesStringContent = contentType == Core.ContentType.Text || contentType == Core.ContentType.Json || contentType == Core.ContentType.Xml;
        UsesFileContent = contentType == Core.ContentType.File;
        UsesFormDataContent = contentType == Core.ContentType.FormData;
    }

    private ContentType GetContentType() {
        if (Options.ReverseContentTypeMap.TryGetValue(ContentType.Value, out var contentType)) {
            return contentType;
        }

        return Core.ContentType.None;
    }

    [RelayCommand]
    public void ToggleHasManualContentTypeHeader()
        => HasManualContentTypeHeader.Value = !HasManualContentTypeHeader.Value;
}
