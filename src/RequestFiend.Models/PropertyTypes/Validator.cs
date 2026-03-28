using RequestFiend.Core;
using System.Linq;

namespace RequestFiend.Models.PropertyTypes;

public static class Validator {
    public static bool Required(string value) => !string.IsNullOrEmpty(value);
    public static bool Numeric(string value) => !string.IsNullOrEmpty(value) && value.All(char.IsAsciiDigit);
    public static bool VariableName(string value) => RequestTemplateCollection.IsValidVariableName(value);
}
