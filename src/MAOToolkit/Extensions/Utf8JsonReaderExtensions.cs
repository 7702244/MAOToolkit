using System.Buffers;
using System.Text;
using System.Text.Json;

namespace MAOToolkit.Extensions;

public static class Utf8JsonReaderExtensions
{
    /// <summary>
    /// Reads the current JSON token as its raw string value.
    /// </summary>
    public static string? GetRawPropertyValue(this Utf8JsonReader jsonReader)
    {
        if (jsonReader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var utf8Bytes = jsonReader.HasValueSequence ? jsonReader.ValueSequence.ToArray() : jsonReader.ValueSpan;

        return System.Text.RegularExpressions.Regex.Unescape(Encoding.UTF8.GetString(utf8Bytes));
    }
}
