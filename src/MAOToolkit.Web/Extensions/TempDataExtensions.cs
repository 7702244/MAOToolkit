using System.Text.Json;
using System.Text.Json.Serialization;
using MAOToolkit.Utilities.JsonConverters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MAOToolkit.Extensions;

public static class TempDataExtensions
{
    private static readonly JsonSerializerOptions JsonSettings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringConverter(), new JsonTimeSpanConverter(), new JsonStringValuesConverter(), new JsonRouteValueDictionaryConverter() },
    };

    public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : new()
    {
        tempData[key] = JsonSerializer.Serialize(value, JsonSettings);
    }

    public static T Get<T>(this ITempDataDictionary tempData, string key) where T : new()
    {
        if (!tempData.TryGetValue(key, out object? obj) || obj is null)
        {
            return new T();
        }

        return JsonSerializer.Deserialize<T>((string)obj, JsonSettings) ?? new T();
    }

    public static T Peek<T>(this ITempDataDictionary tempData, string key) where T : new()
    {
        tempData.Keep(key);

        return tempData.Get<T>(key);
    }
}