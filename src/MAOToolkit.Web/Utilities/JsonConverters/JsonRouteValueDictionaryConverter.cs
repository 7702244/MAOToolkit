using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace MAOToolkit.Utilities.JsonConverters;

public class JsonRouteValueDictionaryConverter : JsonConverter<RouteValueDictionary>
{
    public override RouteValueDictionary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var stringValuesConverter = (JsonConverter<StringValues>)options.GetConverter(typeof(StringValues));

        var routeValues = new RouteValueDictionary();

        while (reader.Read())
        {
            // Exit from object and stop reading.
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return routeValues;
            }

            // Get the key.
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string? propertyName = reader.GetString();
            if (String.IsNullOrEmpty(propertyName))
            {
                throw new JsonException();
            }

            // Get the value.
            reader.Read();
            var value = stringValuesConverter.Read(ref reader, typeof(StringValues), options);

            // Add to dictionary.
            routeValues.Add(propertyName, value);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, RouteValueDictionary value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName
                (options.PropertyNamingPolicy?.ConvertName(kvp.Key) ?? kvp.Key);

            writer.WriteStringValue(kvp.Value?.ToString());
        }

        writer.WriteEndObject();
    }
}