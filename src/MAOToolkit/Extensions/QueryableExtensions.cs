using System.Linq.Expressions;
using System.Reflection;

namespace MAOToolkit.Extensions;

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty, bool desc = false)
    {
        string methodName = desc ? "OrderByDescending" : "OrderBy";

        var type = typeof(T);
        var parameter = Expression.Parameter(type, "p");
        PropertyInfo? property = null;
        Expression propertyAccess = parameter;
        foreach (string propertyName in orderByProperty.Split('.',  StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                break;
            }
                
            type = property.PropertyType;
            propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
        }
            
        ArgumentNullException.ThrowIfNull(property);
            
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);
        object? result = typeof(Queryable).GetMethods().Single(method => method.Name == methodName
                                                                         && method.IsGenericMethodDefinition
                                                                         && method.GetGenericArguments().Length == 2
                                                                         && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.PropertyType)
            .Invoke(null, [source, orderByExpression]);

        if (result is null)
        {
            throw new InvalidOperationException();
        }
            
        return (IOrderedQueryable<T>)result;
    }

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

    private sealed class ParameterSubstitutionVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _destination;
        private readonly ParameterExpression _source;

        public ParameterSubstitutionVisitor(ParameterExpression source, ParameterExpression destination)
        {
            _source = source;
            _destination = destination;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, _source) ? _destination : base.VisitParameter(node);
        }
    }
}