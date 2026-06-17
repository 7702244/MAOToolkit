using System.Reflection;

namespace MAOToolkit.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Performs the specified action on each element of the System.Collections.Generic.IEnumerable.
    /// </summary>
    public static IEnumerable<T> ForEach<T>(
        this IEnumerable<T> source,
        Action<T> act)
    {
        foreach (var element in source)
        {
            act(element);
        }
        return source;
    }
    
    /// <summary>
    /// Performs dynamic sorting of a sequence using the specified property path.
    /// Supports nested properties and safely handles intermediate <c>null</c> values.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source sequence to sort.</param>
    /// <param name="orderByProperty">
    /// The property path to sort by.
    /// Nested properties can be specified using dot notation,
    /// for example: <c>"Name"</c> or <c>"Report.Impressions"</c>.
    /// </param>
    /// <param name="desc">
    /// <c>true</c> to sort in descending order;
    /// otherwise, <c>false</c> to sort in ascending order.
    /// </param>
    /// <returns>
    /// An <see cref="IOrderedEnumerable{T}"/> whose elements are sorted
    /// according to the specified property.
    /// </returns>
    public static IOrderedEnumerable<T> OrderBy<T>(
        this IEnumerable<T> source,
        string orderByProperty,
        bool desc = false)
    {
        var accessor = CreateAccessor<T>(orderByProperty);

        return desc
            ? source.OrderByDescending(accessor)
            : source.OrderBy(accessor);
    }

    private static Func<T, object?> CreateAccessor<T>(string propertyPath)
    {
        string[] properties = propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return item =>
        {
            object? value = item;

            foreach (string propertyName in properties)
            {
                if (value == null)
                    return null;

                var property = value.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                    throw new ArgumentException($"Property '{propertyName}' not found.");

                value = property.GetValue(value);
            }

            return value;
        };
    }
}