using RequestFiend.Models.PropertyTypes;
using System;
using System.Linq;

namespace RequestFiend.Models;

public partial class UrlModel : BoundModelBase {
    public ValidatableProperty<string> BaseUrl { get; }
    public NameValuePairModelCollection Parameters { get; } = new([], Validator.Required);
    public string Url { get => field; private set => SetProperty(ref field, value); }

    public UrlModel(string url) {
        var index = url.IndexOf('?');

        if (index == -1) {
            BaseUrl = new(() => url, Validator.Required);
        }
        else {
            BaseUrl = new(() => url.Substring(0, index));

            foreach (var parameter in url.Substring(index + 1).Split("&", StringSplitOptions.RemoveEmptyEntries)) {
                var valueIndex = parameter.IndexOf('=');

                if (valueIndex == -1) {
                    Parameters.Add(new(parameter, "", Validator.Required));
                }
                else {
                    Parameters.Add(new(parameter.Substring(0, valueIndex), parameter.Substring(valueIndex + 1), Validator.Required));
                }
            }
        }

        if (Parameters.Count > 0) {
            Url = $"{BaseUrl.Value}?{string.Join('&', Parameters.Select(parameter => $"{parameter.Name.Value}={parameter.Value.Value}"))}";
        }
        else {
            Url = BaseUrl.Value;
        }
    }
}
