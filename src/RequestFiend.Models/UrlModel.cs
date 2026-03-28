using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    private Func<string?, CancellationToken, Task> closeMethod;

    public ValidatableProperty<string> BaseUrl { get; set; }
    public NameValuePairModelCollection Parameters { get; } = new([], Validator.Required);

#pragma warning disable CS9264 // Url is set in  UpdateState called by ConfigureState
    public UrlModel(Func<string?, CancellationToken, Task> closeMethod, string url) {
        this.closeMethod = closeMethod;
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

    [RelayCommand]
    public async Task Confirm(CancellationToken cancellationToken) {
        if (HasError) {
            return;
        }
        
        await closeMethod(GetUrl(), cancellationToken);
    }

    [RelayCommand]
    public Task Cancel(CancellationToken cancellationToken)
        => closeMethod(null, cancellationToken);

    private string GetUrl() {
        if (Parameters.Count > 0) {
            return $"{BaseUrl.Value}?{string.Join('&', Parameters.Select(parameter => $"{EncodeUrlComponent(parameter.Name.Value)}={EncodeUrlComponent(parameter.Value.Value)}"))}";
        }
        else {
            return BaseUrl.Value;
        }
    }
}
