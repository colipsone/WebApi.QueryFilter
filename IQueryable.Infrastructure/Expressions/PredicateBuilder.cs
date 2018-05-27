using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Expressions
{
    /// <summary>
    /// Enables the efficient, dynamic composition of query predicates.
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Creates a predicate expression from the specified lambda expression.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate;
        }

        /// <summary>
        /// Creates a predicate that evaluates to false.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return param => false;
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            UnaryExpression negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// Creates a predicate that evaluates to true.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return param => true;
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="merge">The merge.</param>
        /// <returns></returns>
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            Dictionary<ParameterExpression, ParameterExpression> map = first.Parameters
                .Select((f, i) => new {f, s = second.Parameters[i]})
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                if (_map.TryGetValue(p, out ParameterExpression replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }
    }
}