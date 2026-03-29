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
    public NameValuePairModelCollection Parameters { get; }

    public UrlModel(Func<string?, CancellationToken, Task> closeMethod, string url) {
        this.closeMethod = closeMethod;
        var (baseUrl, parameters) = ParseUrl(url);

        BaseUrl = new(() => baseUrl, Validator.Required);
        Parameters = new([.. parameters.Select(parameter => new NameValuePair() { Name = parameter.Name, Value = parameter.Value })], Validator.Required);

        ConfigureState([BaseUrl, Parameters]);
    }

    [RelayCommand]
    public void ParseQueryStringFromBaseUrl() {
        var (baseUrl, parameters) = ParseUrl(BaseUrl.Value);

        BaseUrl.Value = baseUrl;

        foreach (var parameter in parameters) {
            Parameters.Add(parameter.Name, parameter.Value);
        }
    }

    private (string BaseUrl, List<(string Name, string Value)> Parameters) ParseUrl(string url) {
        var index = url.IndexOf('?');

        if (index == -1) {
            return (url, []);
        }
        else {
            var baseUrl = url.Substring(0, index);
            var parameters = new List<(string Name, string Value)>();

            foreach (var parameter in url.Substring(index + 1).Split("&", StringSplitOptions.RemoveEmptyEntries)) {
                var valueIndex = parameter.IndexOf('=');

                if (valueIndex == -1) {
                    parameters.Add((HttpUtility.UrlDecode(parameter), ""));
                }
                else {
                    parameters.Add((HttpUtility.UrlDecode(parameter.Substring(0, valueIndex)), HttpUtility.UrlDecode(parameter.Substring(valueIndex + 1))));
                }
            }

            return (baseUrl, parameters);
        }
    }

    [RelayCommand]
    public async Task Confirm(CancellationToken cancellationToken) {
        if (HasError) {
            return;
        }

        var url = BaseUrl.Value;

        if (Parameters.Count > 0) {
            var index = url.IndexOf('?');

            if (index == -1) {
                url += "?";
            }
            else if (index < url.Length - 1) {
                url += "&";
            }

            url += $"{string.Join('&', Parameters.Select(parameter => $"{EncodeUrlComponent(parameter.Name.Value)}={EncodeUrlComponent(parameter.Value.Value)}"))}";
        }

        await closeMethod(url, cancellationToken);
    }

    [RelayCommand]
    public Task Cancel(CancellationToken cancellationToken)
        => closeMethod(null, cancellationToken);
}
