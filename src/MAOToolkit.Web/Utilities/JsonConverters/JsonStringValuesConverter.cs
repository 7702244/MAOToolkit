using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Primitives;

namespace MAOToolkit.Utilities.JsonConverters;

public class JsonStringValuesConverter : JsonConverter<StringValues>
{
    public override StringValues Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? s = reader.GetString();
        if (!String.IsNullOrEmpty(s))
        {
            return new StringValues(s.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, StringValues value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}