using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAOToolkit.Utilities.JsonConverters;

public sealed class JsonCamelCaseEnumConverter<TEnum>() : JsonStringEnumConverter<TEnum>(JsonNamingPolicy.CamelCase)
    where TEnum : struct, Enum;