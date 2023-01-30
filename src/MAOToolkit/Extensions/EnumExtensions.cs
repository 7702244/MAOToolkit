using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MAOToolkit.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets name from <see cref="DisplayAttribute"/>.
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            if (!type.IsEnum)
                throw new ArgumentException($"Type '{type}' is not Enum");

            var members = type.GetMember(value.ToString());
            if (members.Length == 0)
                return value.ToString();
            //throw new ArgumentException($"Member '{value}' not found in type '{type.Name}'");

            return members[0].GetCustomAttribute<DisplayAttribute>(false)?.GetName() ?? value.ToString();
        }

        /// <summary>
        /// Gets full type name with namespace.
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns></returns>
        public static string GetFullName(this Enum value)
        {
            return $"{value.GetType().Name}.{value}";
        }
    }
}