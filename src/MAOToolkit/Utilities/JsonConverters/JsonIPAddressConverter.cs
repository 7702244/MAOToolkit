using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAOToolkit.Utilities.JsonConverters;

/// <summary>
/// Converts IP addresses to and from JSON strings.
/// </summary>
public sealed class JsonIPAddressConverter : JsonConverter<IPAddress>
{
    /// <summary>
    /// Reads a value from JSON.
    /// </summary>
    public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (IPAddress.TryParse(reader.GetString(), out IPAddress? result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Writes a value to JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
