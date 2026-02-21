using System.Linq;

namespace RequestFiend.Models.PropertyTypes;

public static class Validator {
    public static bool Required(string value) => !string.IsNullOrEmpty(value);
    public static bool Numeric(string value) => !string.IsNullOrEmpty(value) && !value.Any(c => !char.IsAsciiDigit(c));
}
