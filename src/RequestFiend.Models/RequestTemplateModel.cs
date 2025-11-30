using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace RequestFiend.Models;

public class RequestTemplateModel : BoundModelBase {
    private static JsonSerializerOptions JsonSerializerOptions { get; } = new() { WriteIndented = true };

    public ValidatableString Name { get; set; }
    public ValidatableString Method { get; set; }
    public ValidatableString Url { get; set; }
    public NameValuePairModelCollection Headers { get; set; }
    public ContentType ContentType {
        get => field;
        set {
            if (SetProperty(ref field, value)) {
                UsesStringContent = field is ContentType.Text or ContentType.Json;
                UsesJsonContent = field is ContentType.Json;
            }
        }
    }
    public bool UsesStringContent {
        get => field;
        set => SetProperty(ref field, value);
    }
    public bool UsesJsonContent {
        get => field;
        set => SetProperty(ref field, value);
    }
    public ValidatableString StringContent { get; set; }
    public bool IsModified {
        get => field;
        set => SetProperty(ref field, value);
    }
    public bool HasError {
        get => field;
        set => SetProperty(ref field, value);
    }

    public RequestTemplateModel(RequestTemplate request) {
        Name = new(true, () => request.Name);
        Method = new(true, () => request.Method);
        Url = new(true, () => request.Url);
        Headers = new(request.Headers);
        ContentType = request.ContentType;
        StringContent = new(false, () => request.StringContent);
        HasError = Name.HasError || Method.HasError || Url.HasError || Headers.HasError || StringContent.HasError;
        IsModified = Name.IsModified || Method.IsModified || Url.IsModified || Headers.IsModified || StringContent.IsModified;

        Name.PropertyChanged += OnPropertyChanged;
        Method.PropertyChanged += OnPropertyChanged;
        Url.PropertyChanged += OnPropertyChanged;
        ((INotifyPropertyChanged)Headers).PropertyChanged += OnPropertyChanged;
        StringContent.PropertyChanged += OnPropertyChanged;
    }

    public bool TryUpdateRequestTemplate(RequestTemplate request) {
        if (Name.HasError || Method.HasError || Url.HasError || Headers.HasError) {
            return false;
        }

        request.Name = Name.Value!;
        request.Method = Method.Value!;
        request.Url = Url.Value!;
        request.Headers = [.. Headers.Select(header => new NameValuePair() { Name = header.Name.Value!, Value = header.Value.Value! })];
        request.ContentType = ContentType;
        request.StringContent = StringContent.Value;

        Name.IsModified = false;
        Method.IsModified = false;
        Url.IsModified = false;
        Headers.Reinitialize(request.Headers);
        StringContent.IsModified = false;

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
                StringContent.Value = JsonSerializer.Serialize(document, JsonSerializerOptions);
            }
            exception = null;
            return true;
        }
        catch (Exception ex) {
            exception = ex;
            return false;
        }
    }

    private void OnPropertyChanged(object? _, PropertyChangedEventArgs e) {
        if (e.PropertyName == Constants.IsModified) {
            IsModified = Name.IsModified || Method.IsModified || Url.IsModified || Headers.IsModified || StringContent.IsModified;
        }
        if (e.PropertyName == Constants.HasError) {
            HasError = Name.HasError || Method.HasError || Url.HasError || Headers.HasError || StringContent.HasError;
        }
    }
}
