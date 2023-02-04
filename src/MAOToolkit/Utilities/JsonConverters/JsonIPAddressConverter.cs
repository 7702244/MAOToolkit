using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAOToolkit.Utilities.JsonConverters
{
    public class JsonIPAddressConverter : JsonConverter<IPAddress>
    {
        public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (IPAddress.TryParse(reader.GetString(), out IPAddress? result))
            {
                return result;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}