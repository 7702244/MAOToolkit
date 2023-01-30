using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAOToolkit.Utilities.JsonConverters
{
    public class JsonCamelCaseEnumConverter : JsonStringEnumConverter
    {
        public JsonCamelCaseEnumConverter() : base(JsonNamingPolicy.CamelCase) { }
    }
}