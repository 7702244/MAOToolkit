using System.Text.Json;
using System.Text.Json.Serialization;
using MAOToolkit.Extensions;

namespace MAOToolkit.Utilities.JsonConverters
{
    /// <summary>
    /// Gives opportunity for deserializing non-string primitives like
    /// Int32, Bool, Double, etc. properties into String properties.
    /// </summary>
    public class JsonStringConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetRawPropertyValue();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}