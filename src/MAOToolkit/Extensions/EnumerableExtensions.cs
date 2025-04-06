using System.Linq.Expressions;
using System.Reflection;

namespace MAOToolkit.Extensions
{
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
        
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string orderByProperty, bool desc = false)
        {
            string methodName = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(T);
            var property = type.GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            ArgumentNullException.ThrowIfNull(property);

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            
            object? result = typeof(Enumerable).GetMethods().Single(method => method.Name == methodName
                && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2
                && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.PropertyType)
            .Invoke(null, [source, orderByExpression.Compile()]);

            if (result is null)
            {
                throw new InvalidOperationException();
            }
            
            return (IOrderedEnumerable<T>)result;
        }
    }
}