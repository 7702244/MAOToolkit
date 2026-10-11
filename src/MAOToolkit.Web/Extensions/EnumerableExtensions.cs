using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace MAOToolkit.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Creates route values from key-value pairs and an optional object.
    /// </summary>
    public static RouteValueDictionary ToRouteValues(this IEnumerable<KeyValuePair<string, StringValues>> col, object? obj = null)
    {
        var values = new RouteValueDictionary(obj);
        foreach (var kvp in col)
        {
            if (!String.IsNullOrEmpty(kvp.Key))
                values.TryAdd(kvp.Key, kvp.Value);
        }
        return values;
    }
}
