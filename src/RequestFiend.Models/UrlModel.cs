using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequestFiend.Models;

public partial class UrlModel : BoundModelBase {
    public static string EncodeUrlComponent(string urlComponent) {
        if (urlComponent.Length == 0) {
            return urlComponent;
        }

        var chunks = new List<(string Value, bool NeedsEncoding)>();
        var previousPosition = 0;

        for (var position = 0; position < urlComponent.Length; position++) {
            if (TryGetVariableReferenceLength(urlComponent, position, out var length)) {
                chunks.Add((urlComponent.Substring(previousPosition, position - previousPosition), true));
                chunks.Add((urlComponent.Substring(position, length), false));
                previousPosition = position + length;
            }
        }

        chunks.Add((urlComponent.Substring(previousPosition), true));

        return string.Join("", chunks.Select(chunk => chunk.NeedsEncoding ? HttpUtility.UrlEncode(chunk.Value) : chunk.Value));
    }

    private static bool TryGetVariableReferenceLength(string urlComponent, int position, out int length) {
        if (urlComponent.Length > position + 4 && urlComponent[position] == '{' && urlComponent[position + 1] == '{') {
            length = 4;

            while (urlComponent.Length > position + length && RequestTemplateCollection.IsValidVariableCharacter(urlComponent[position + length - 2])) {
                length++;
            }

            return urlComponent[position + length - 1] == '}' && urlComponent[position + length - 2] == '}';
        }

        length = 0;
        return false;
    }

    public ValidatableProperty<string> BaseUrl { get; set; }
    public NameValuePairModelCollection Parameters { get; } = new([], Validator.Required);
    public string Url { get => field; private set => SetProperty(ref field, value); }

#pragma warning disable CS9264 // Url is set in  UpdateState called by ConfigureState
    public UrlModel(string url) {
        BaseUrl = new(() => url, Validator.Required);

        ParseQueryStringFromBaseUrl();
        ConfigureState([BaseUrl, Parameters]);
    }
#pragma warning restore CS9264 // Url is set in UpdateState called by ConfigureState

    [RelayCommand]
    public void ParseQueryStringFromBaseUrl() {
        var url = BaseUrl.Value;
        var index = url.IndexOf('?');

        if (index == -1) {
            BaseUrl.Reset(() => url);
        }
        else {
            BaseUrl.Reset(() => url.Substring(0, index));

            foreach (var parameter in url.Substring(index + 1).Split("&", StringSplitOptions.RemoveEmptyEntries)) {
                var valueIndex = parameter.IndexOf('=');

                if (valueIndex == -1) {
                    Parameters.Add(HttpUtility.UrlDecode(parameter), "");
                }
                else {
                    Parameters.Add(HttpUtility.UrlDecode(parameter.Substring(0, valueIndex)), HttpUtility.UrlDecode(parameter.Substring(valueIndex + 1)));
                }
            }
        }

        UpdateState();
    }

    protected override void UpdateState() {
        if (Parameters.Count > 0) {
            Url = $"{BaseUrl.Value}?{string.Join('&', Parameters.Select(parameter => $"{EncodeUrlComponent(parameter.Name.Value)}={EncodeUrlComponent(parameter.Value.Value)}"))}";
        }
        else {
            Url = BaseUrl.Value;
        }
    }
}
