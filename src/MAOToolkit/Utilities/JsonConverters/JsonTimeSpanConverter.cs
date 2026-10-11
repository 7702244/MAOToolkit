using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAOToolkit.Utilities.JsonConverters;

/// <summary>
/// Converts time spans to and from JSON strings.
/// </summary>
public sealed class JsonTimeSpanConverter : JsonConverter<TimeSpan>
{
    /// <summary>
    /// Reads a value from JSON.
    /// </summary>
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (TimeSpan.TryParse(reader.GetString(), out TimeSpan result))
        {
            return result;
        }

        return default;
    }

    /// <summary>
    /// Writes a value to JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
