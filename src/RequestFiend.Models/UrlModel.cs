using CommunityToolkit.Mvvm.Input;
using RequestFiend.Core;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RequestFiend.Models;

public partial class UrlModel : BoundModelBase {
    private static string EncodeUrlComponent(string urlComponent) {
        return string.Join("", VariableService.ProcessText(urlComponent, HttpUtility.UrlEncode, variableReference => variableReference));
    }

    private readonly Func<string?, CancellationToken, Task> closeMethod;

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

        foreach (var (Name, Value) in parameters) {
            Parameters.Add(Name, Value);
        }
    }

    private static (string BaseUrl, List<(string Name, string Value)> Parameters) ParseUrl(string url) {
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
