using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace RequestFiend.Models.Converters;

public class SelectedItemStyleConverter : IMultiValueConverter {
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        if (values.Length < 3 || values[2] is not Style style || !Equals(values[0], values[1])) {
            return null;
        }

        return style;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}