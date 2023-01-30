using System.Linq.Expressions;

namespace MAOToolkit.Extensions
{
    public static class QueryableExtensions
    {
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
}