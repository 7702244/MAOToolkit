using System.Buffers;
using System.Text;
using System.Text.Json;

namespace MAOToolkit.Extensions
{
    public static class Utf8JsonReaderExtensions
    {
        public static string? GetRawPropertyValue(this Utf8JsonReader jsonReader)
        {
            if (jsonReader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            ReadOnlySpan<byte> utf8Bytes = jsonReader.HasValueSequence ? jsonReader.ValueSequence.ToArray() : jsonReader.ValueSpan;

            return System.Text.RegularExpressions.Regex.Unescape(Encoding.UTF8.GetString(utf8Bytes));
        }
    }
}