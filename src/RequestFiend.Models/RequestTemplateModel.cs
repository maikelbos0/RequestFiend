using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RequestFiend.Core;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestTemplateModel : BoundModelBase {
    private readonly static JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    private readonly IRequestTemplateCollectionService requestTemplateCollectionService;
    private readonly IPopupService popupService;
    private readonly IMessageService messageService;
    private readonly RequestTemplate request;

    public ValidatableString Name { get; set; }
    public ValidatableString Method { get; set; }
    public ValidatableString Url { get; set; }
    public NameValuePairModelCollection Headers { get; set; }
    public ValidatableString ContentType { get; set; }
    public bool UsesStringContent {
        get => field;
        private set => SetProperty(ref field, value);
    }
    public bool UsesJsonContent {
        get => field;
        private set => SetProperty(ref field, value);
    }
    public ValidatableString StringContent { get; set; }


    public RequestTemplateModel(IRequestTemplateCollectionService requestTemplateCollectionService, IPopupService popupService, IMessageService messageService, RequestTemplate request) {
        this.requestTemplateCollectionService = requestTemplateCollectionService;
        this.popupService = popupService;
        this.messageService = messageService;
        this.request = request;

        Name = new(true, () => request.Name);
        Method = new(true, () => request.Method);
        Url = new(true, () => request.Url);
        Headers = new(request.Headers);
        ContentType = new(true, () => Options.ContentTypeMap[request.ContentType]);
        StringContent = new(false, () => request.StringContent);

        ContentType.PropertyChanged += OnContentTypeChanged;
        UsesStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text] || ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
        UsesJsonContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
    }

    public bool TryUpdateRequestTemplate(RequestTemplate request) {
        if (Name.HasError || Method.HasError || Url.HasError || Headers.Any(header => header.Name.HasError || header.Value.HasError) || ContentType.HasError) {
            return false;
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
        Headers.Reinitialize(request.Headers);
        ContentType.Reset();
        StringContent.Reset();

        return true;
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
            requestTemplateCollectionService.Collection.Requests.Remove(request);
            await requestTemplateCollectionService.Save();
            messageService.Send(new RequestTemplateDeletedMessage(), request.Id);
        }
    }

    private void OnContentTypeChanged(object? sender, PropertyChangedEventArgs e) {
        UsesStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text] || ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
        UsesJsonContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
    }
}
