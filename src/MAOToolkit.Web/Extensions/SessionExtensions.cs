using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace MAOToolkit.Extensions;

public static class SessionExtensions
{
    private static readonly JsonSerializerOptions JsonSettings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public static void SetObject(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value, JsonSettings));
    }

    public static T? GetObject<T>(this ISession session, string key)
    {
        string? value = session.GetString(key);
        if (value is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value, JsonSettings);
    }

    public static T? GetOrAdd<T>(this ISession session, string key, Func<T> getter)
    {
        T? value;

        if (!session.Keys.Contains(key))
        {
            value = getter();

            session.SetObject(key, value!);
        }
        else
        {
            value = session.GetObject<T>(key);
        }

        return value;
    }

    public static async Task<T?> GetOrAdd<T>(this ISession session, string key, Func<Task<T>> getter)
    {
        T? value;

        if (!session.Keys.Contains(key))
        {
            value = await getter();

            session.SetObject(key, value!);
        }
        else
        {
            value = session.GetObject<T>(key);
        }

        return value;
    }
}