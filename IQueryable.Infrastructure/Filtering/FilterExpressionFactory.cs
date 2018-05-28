using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IQueryableFilter.Infrastructure.Caching;
using IQueryableFilter.Infrastructure.Common;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class FilterExpressionFactory : IFilterExpressionFactory
    {
        private readonly ICacheManager _cacheManager;
        private readonly IFilterValueParserFactory _filterValueParserFactory;
        private readonly IPropertyInfoResolver _propertyInfoResolver;

        public FilterExpressionFactory(ICacheManager cacheManager,
            IFilterValueParserFactory filterValueParserFactory,
            IPropertyInfoResolver propertyInfoResolver)
        {
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
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

            return Expression.Lambda<Func<TEntity, bool>>(queryFilters.Select(filter => GetFilterExpression<TEntity>(filter, entityParam))
                .Aggregate(Expression.And), entityParam);
        }

        private Expression GetFilterExpression<TEntity>(Filter filter, Expression entityParam) where TEntity : class, new()
        {
            PropertyInfo propertyInfo = _propertyInfoResolver.GetPropertyInfo<TEntity>(filter.PropertyName);

            return Expression.Equal(Expression.Property(entityParam, filter.PropertyName),
                Expression.Constant(_filterValueParserFactory.GetValueParser(propertyInfo).Parse(filter.Value),
                    propertyInfo.PropertyType));
        }

        /*private Dictionary<string, IValueParser> GetPropertyFieldParsers<TEntity>(Filter[] queryFilters,
            string filterSetUniqueName)
        {
            return _cacheManager.GetOrAdd(Constants.CacheKaeys.FilterSetUniqueNameTemplate + filterSetUniqueName,
                () => queryFilters.ToDictionary(filter => filter.PropertyName,
                    filter =>
                    {
                        PropertyInfo prop = _propertyInfoResolver.GetPropertyInfo<TEntity>(filter.PropertyName);

                        if (prop == null)
                            throw new ArgumentException(
                                $"Filtered entity has no field with name {filter.PropertyNameSource}.");

                        return _filterValueParserFactory.GetValueParser(prop);
                    }),
                null);
        }

        private object[] ParseFilterValues<TEntity>(IFiltering filtering)
        {
            Filter[] queryFilters = filtering.GetQueryFilters();
            Dictionary<string, IValueParser> fieldParsers =
                GetPropertyFieldParsers<TEntity>(queryFilters, filtering.GetFilterSetUniqueName());

            return queryFilters.Select(filter =>
                {
                    try
                    {
                        return fieldParsers[filter.PropertyName].Parse(filter.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(
                            $"Inappropriate filter value '{filter.Value}' for field with name {filter.PropertyNameSource}.",
                            ex);
                    }
                })
                .ToArray();
        }*/
    }
}