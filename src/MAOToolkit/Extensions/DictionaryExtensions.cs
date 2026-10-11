namespace MAOToolkit.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Returns the existing value or adds a value created by the factory.
    /// </summary>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> getter) where TKey : notnull
    {
        if (!dictionary.TryGetValue(key, out TValue? value))
        {
            value = getter();
            dictionary.Add(key, value);
        }

        return value;
    }

    /// <summary>
    /// Returns the existing value or adds a value created by the factory.
    /// </summary>
    public static async Task<TValue> GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<Task<TValue>> getter) where TKey : notnull
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = await getter();
            dictionary.Add(key, value);
        }

        return value;
    }

    /// <summary>
    /// Returns the existing value or adds a value created by the factory.
    /// </summary>
    public static TValue? GetOrAdd<TValue>(this IDictionary<object, object?> dictionary, object key, Func<TValue> getter)
    {
        if (!dictionary.TryGetValue(key, out object? value))
        {
            value = getter();
            dictionary.Add(key, value);
        }

        return (TValue?)value;
    }

    /// <summary>
    /// Returns the existing value or adds a value created by the factory.
    /// </summary>
    public static async Task<TValue?> GetOrAdd<TValue>(this IDictionary<object, object?> dictionary, object key, Func<Task<TValue>> getter)
    {
        if (!dictionary.TryGetValue(key, out object? value))
        {
            value = await getter();
            dictionary.Add(key, value);
        }

        return (TValue?)value;
    }
}
