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

        public Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(IFiltering filtering)
            where TEntity : class, new()
        {
            Filter[] queryFilters = filtering.GetQueryFilters();
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
            object parsedValue;
            try
            {
                parsedValue = valueParser.Parse(filter.Value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"Inappropriate filter value '{filter.Value}' for field with name {filter.PropertyNameSource}.",
                    ex);
            }

            return Expression.Equal(Expression.Property(entityParam, filter.PropertyName), Expression.Constant(
                parsedValue,
                propertyInfo.PropertyType));
        }
    }
}