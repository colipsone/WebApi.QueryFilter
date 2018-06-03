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
                throw new ArgumentException($"Filtered entity has no field with name {filter.PropertyName}.");

            IValueParser valueParser = _filterValueParserFactory.GetValueParser(propertyInfo);

            if (!valueParser.TryParse(filter.Value, out object parsedValue))
            {
                throw new ArgumentException(
                    $"Inappropriate filter value '{filter.Value}' for field with name {filter.PropertyName}.");
            }

            return filter.Operation.GetOperationExpression(Expression.Property(entityParam, propertyInfo.Name), Expression.Constant(
                    parsedValue,
                    propertyInfo.PropertyType));
        }
    }
}