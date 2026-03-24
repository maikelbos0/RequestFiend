using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Linq;

namespace RequestFiend.Models.Converters;

public class ByteArrayToHexStringConverter : IValueConverter {
    private static readonly string hexStringLookup = System.Convert.ToHexString([.. Enumerable.Range(byte.MinValue, byte.MaxValue - byte.MinValue + 1).Select(i => (byte)i)]);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is not byte[] byteArray) {
            return null;
        }

        const int spacePeriod = 4;
        var characters = new char[byteArray.Length * 2 + (byteArray.Length - 1) / 4];
        var spaceCount = 0;

        for (int i = 0; i < byteArray.Length; i++) {
            if (i > 0 && i % spacePeriod == 0) {
                characters[i * 2 + spaceCount++] = ' ';
            }

            characters[i * 2 + spaceCount] = hexStringLookup[byteArray[i] * 2];
            characters[i * 2 + spaceCount + 1] = hexStringLookup[byteArray[i] * 2 + 1];
        }

        return new string(characters);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
