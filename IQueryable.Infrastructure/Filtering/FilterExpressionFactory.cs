using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class FilterExpressionFactory : IFilterExpressionFactory
    {
        private readonly IFilterValueParserFactory _filterValueParserFactory;
        private readonly IPropertyInfoResolver _propertyInfoResolver;

        public FilterExpressionFactory(IFilterValueParserFactory filterValueParserFactory,
            IPropertyInfoResolver propertyInfoResolver)
        {
            _filterValueParserFactory = filterValueParserFactory ??
                                        throw new ArgumentNullException(nameof(filterValueParserFactory));
            _propertyInfoResolver =
                propertyInfoResolver ?? throw new ArgumentNullException(nameof(propertyInfoResolver));
        }

        public Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(Filter[] queryFilters)
            where TEntity : class, new()
        {
            if (!queryFilters.Any()) return entity => true;

            ParameterExpression entityParam = Expression.Parameter(typeof(TEntity));

            return Expression.Lambda<Func<TEntity, bool>>(queryFilters
                .Select(filter => GetFilterExpression<TEntity>(filter, entityParam))
                .Aggregate(Expression.And), entityParam);
        }

        private Expression GetFilterExpression<TEntity>(Filter filter, Expression entityParam)
            where TEntity : class, new()
        {
            PropertyInfo propertyInfo = _propertyInfoResolver.GetPropertyInfo<TEntity>(filter.PropertyName);

            if (propertyInfo == null)
                throw new ArgumentException($"Filtered entity has no field with name {filter.PropertyNameSource}.");

            IValueParser valueParser = _filterValueParserFactory.GetValueParser(propertyInfo);

            if (!valueParser.TryParse(filter.Value, out object parsedValue))
            {
                throw new ArgumentException(
                    $"Inappropriate filter value '{filter.Value}' for field with name {filter.PropertyNameSource}.");
            }

            return GetExpressionByOperation(filter.Operation, Expression.Property(entityParam, filter.PropertyName),
                Expression.Constant(
                    parsedValue,
                    propertyInfo.PropertyType));
        }

        private static Expression GetExpressionByOperation(FilterOperation operation, Expression left, Expression right)
        {
            switch (operation)
            {
                case FilterOperation.Equals:
                    return Expression.Equal(left, right);
                case FilterOperation.GreaterThan:
                    return Expression.GreaterThan(left, right);
                case FilterOperation.LessThan:
                    return Expression.LessThan(left, right);
                case FilterOperation.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left, right);
                case FilterOperation.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left, right);
                case FilterOperation.Named:
                    throw new NotSupportedException("Named filter is not supported in this context!");
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }
    }
}