using System.Reflection;
using System.Runtime.CompilerServices;

namespace MAOToolkit.Extensions;

public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the type is an anonymous object.
    /// </summary>
    public static bool IsAnonymousType(this Type type)
    {
        bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
        bool nameContainsAnonymousType = type.FullName?.Contains("AnonymousType") == true;
        bool isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

        return isAnonymousType;
    }

    /// <summary>
    /// Determines whether the type is simple or not.
    /// </summary>
    public static bool IsSimpleType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // nullable type, check if the nested type is simple.
            return IsSimpleType(type.GetGenericArguments()[0]);
        }

        return type.IsPrimitive
               || type.IsValueType
               || type == typeof(string);
    }

    /// <summary>
    /// Gets the fields of the nested object graph.
    /// </summary>
    public static IEnumerable<FieldInfo> GetNestedFields(this Type type)
    {
        var fields = type.GetFields().ToList();

        foreach (var item in type.GetNestedTypes())
        {
            fields.AddRange(item.GetNestedFields());
        }

        return fields;
    }

    /// <summary>
    /// Returns default value for this type.
    /// </summary>
    /// <returns>Default value for type.</returns>
    public static object? GetDefaultValue(this Type type)
    {
        if (type.IsValueType)
            return Activator.CreateInstance(type);

        return null;
    }
}