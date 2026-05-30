using RequestFiend.Core;
using System;
using System.IO;
using System.Linq;

namespace RequestFiend.Models.PropertyTypes;

public static class Validator {
    public static bool Required(string value) => !string.IsNullOrEmpty(value);
    public static bool FilePath(string value) => File.Exists(value);
    public static bool Numeric(string value) => value.All(char.IsAsciiDigit);
    public static bool VariableName(string value) => RequestTemplateCollection.IsValidVariableName(value);
    public static Func<string, bool> Conditional(Func<bool> isRequired, Func<string, bool> validator) => value => !isRequired() || validator(value);
}
