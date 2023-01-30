using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAOToolkit.Utilities.JsonConverters
{
    public class JsonTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (TimeSpan.TryParse(reader.GetString(), out TimeSpan result))
            {
                return result;
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}