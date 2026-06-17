using System.Linq.Expressions;
using System.Reflection;

namespace MAOToolkit.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Performs dynamic sorting of a queryable sequence using the specified property path.
    /// Supports nested properties and generates an expression tree that can be translated
    /// by LINQ providers such as Entity Framework.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The source query to sort.</param>
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
    /// An <see cref="IOrderedQueryable{T}"/> whose elements are ordered
    /// according to the specified property.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when one or more members in the specified property path cannot be found.
    /// </exception>
    public static IOrderedQueryable<T> OrderBy<T>(
        this IQueryable<T> source,
        string orderByProperty,
        bool desc = false)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (String.IsNullOrWhiteSpace(orderByProperty))
        {
            throw new ArgumentException(
                "Property path cannot be null or empty.",
                nameof(orderByProperty));
        }
        
        var parameter = Expression.Parameter(typeof(T), "p");

        Expression propertyAccess = parameter;
        
        string[] parts = orderByProperty.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string propertyName in parts)
        {
            propertyAccess = Expression.PropertyOrField(propertyAccess, propertyName);
        }

        // For LINQ to Objects, we wrap the expression in null checks starting from the second-to-last node.
        // For EF Core, this is not necessary.
        if (source.Provider is EnumerableQuery
            && parts.Length > 1)
        {
            var resultExpression = propertyAccess;

            Expression parent = parameter;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                parent = Expression.PropertyOrField(parent, parts[i]);

                bool canBeNull =
                    !parent.Type.IsValueType ||
                    Nullable.GetUnderlyingType(parent.Type) != null;

                if (!canBeNull)
                    continue;

                var resultType = resultExpression.Type;

                if (resultType.IsValueType &&
                    Nullable.GetUnderlyingType(resultType) == null)
                {
                    resultType = typeof(Nullable<>).MakeGenericType(resultType);
                    resultExpression = Expression.Convert(resultExpression, resultType);
                }

                resultExpression = Expression.Condition(
                    Expression.Equal(
                        parent,
                        Expression.Constant(null, parent.Type)),
                    Expression.Constant(null, resultType),
                    resultExpression);
            }

            propertyAccess = resultExpression;
        }

        var lambda = Expression.Lambda(propertyAccess, parameter);

        var method = desc
            ? OrderByDescendingMethod
            : OrderByMethod;

        object? result = method
            .MakeGenericMethod(typeof(T), propertyAccess.Type)
            .Invoke(null, [source, lambda]);

        return (IOrderedQueryable<T>)result!;
    }
    
    private static readonly MethodInfo OrderByMethod =
        typeof(Queryable)
            .GetMethods()
            .Single(m =>
                m.Name == nameof(Queryable.OrderBy) &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2);

    private static readonly MethodInfo OrderByDescendingMethod =
        typeof(Queryable)
            .GetMethods()
            .Single(m =>
                m.Name == nameof(Queryable.OrderByDescending) &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2);

    public static IQueryable<T> WhereAny<T>(this IQueryable<T> queryable, params Expression<Func<T, bool>>[] predicates)
    {
        var parameter = Expression.Parameter(typeof(T));
        return queryable.Where(Expression.Lambda<Func<T, bool>>(predicates.Aggregate<Expression<Func<T, bool>>, Expression>(Expression.Constant(false),
                (current, predicate) =>
                {
                    var visitor = new ParameterSubstitutionVisitor(predicate.Parameters[0], parameter);
                    return Expression.OrElse(current, visitor.Visit(predicate.Body));
                }),
            parameter));
    }

    private sealed class ParameterSubstitutionVisitor(ParameterExpression source, ParameterExpression destination)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, source) ? destination : base.VisitParameter(node);
        }
    }
}