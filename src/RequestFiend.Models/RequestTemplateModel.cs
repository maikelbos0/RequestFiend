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
using System.Text.Json;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class RequestTemplateModel : RequestTemplateCollectionModelBase {
    private readonly static JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    private readonly IPopupService popupService;
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


    public RequestTemplateModel(IFileService fileService, IPopupService popupService, string filePath, RequestTemplateCollection collection, RequestTemplate request) : base(fileService, filePath, collection) {
        this.popupService = popupService;
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
        StringContent.Reset();

        return true;
    }

    public bool ValidateJson([NotNullWhen(false)] out Exception? exception) {
        try {
            if (!string.IsNullOrEmpty(StringContent.Value)) {
                _ = JsonDocument.Parse(StringContent.Value ?? "");
            }
            exception = null;
            return true;
        }
        catch (Exception ex) {
            exception = ex;
            return false;
        }
    }

    public bool FormatJson([NotNullWhen(false)] out Exception? exception) {
        try {
            if (!string.IsNullOrEmpty(StringContent.Value)) {
                var document = JsonDocument.Parse(StringContent.Value ?? "");
                StringContent.Value = JsonSerializer.Serialize(document, jsonSerializerOptions);
            }
            exception = null;
            return true;
        }
        catch (Exception ex) {
            exception = ex;
            return false;
        }
    }

    [RelayCommand]
    public async Task Delete() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to delete this request?")) {
            collection.Requests.Remove(request);
            await SaveCollection();
            WeakReferenceMessenger.Default.Send(new RequestTemplateDeletedMessage(), request.Id);
        }
    }

    private void OnContentTypeChanged(object? sender, PropertyChangedEventArgs e) {
        UsesStringContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Text] || ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
        UsesJsonContent = ContentType.Value == Options.ContentTypeMap[Core.ContentType.Json];
    }
}
