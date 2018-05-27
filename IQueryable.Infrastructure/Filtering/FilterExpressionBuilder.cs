using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IQueryableFilter.Infrastructure.Caching;
using IQueryableFilter.Infrastructure.Common;
using IQueryableFilter.Infrastructure.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class FilterExpressionBuilder : IFilterExpressionBuilder
    {
        private readonly ICacheManager _cacheManager;
        private readonly IFilterValueParserFactory _filterValueParserFactory;
        private readonly INamedFilterPredicateFactory _namedFilterPredicateFactory;
        private readonly IPropertyInfoResolver _propertyInfoResolver;

        public FilterExpressionBuilder(ICacheManager cacheManager,
            INamedFilterPredicateFactory namedFilterPredicateFactory,
            IFilterValueParserFactory filterValueParserFactory,
            IPropertyInfoResolver propertyInfoResolver)
        {
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
            _namedFilterPredicateFactory = namedFilterPredicateFactory ?? throw new ArgumentNullException(nameof(namedFilterPredicateFactory));
            _filterValueParserFactory = filterValueParserFactory ?? throw new ArgumentNullException(nameof(filterValueParserFactory));
            _propertyInfoResolver = propertyInfoResolver ?? throw new ArgumentNullException(nameof(propertyInfoResolver));
        }

        public Expression<Func<TEntity, bool>> GetNamedFiltersExpressionPredicate<TEntity>(IFiltering filtering) where TEntity : class, new()
        {
            Filter[] namedFilters = filtering.GetNamedFilters();

            if (namedFilters.Length == 0)
            {
                return entity => true;
            }

            return _namedFilterPredicateFactory.GetNamedFilterPredicates<TEntity>(namedFilters).Aggregate((current, predicate) => current.Or(predicate));
        }

        public Expression<Func<TEntity, bool>> GetQueryFiltersExpressionPredicate<TEntity>(IFiltering filtering) where TEntity : class, new()
        {
            Filter[] queryFilters = filtering.GetQueryFilters();
            if (!queryFilters.Any())
            {
                return entity => true;
            }

            object[] values = ParseFilterValues<TEntity>(filtering);

            string expression = string.Join(" and ", queryFilters.Select((filter, i) => filter.ToParameterizedExpression(i)));

            Expression<Func<TEntity, bool>> filterPredicate =
                System.Linq.Dynamic.DynamicExpression.ParseLambda<TEntity, bool>(expression, values);

            return filterPredicate;
        }

        private Dictionary<string, IValueParser> GetPropertyFieldParsers<TEntity>(Filter[] queryFilters, string filterSetUniqueName)
        {
            return _cacheManager.GetOrAdd(Constants.CacheKaeys.FilterSetUniqueNameTemplate + filterSetUniqueName,
                () => queryFilters.ToDictionary(filter => filter.PropertyName,
                    filter =>
                    {
                        PropertyInfo prop = _propertyInfoResolver.GetPropertyInfo<TEntity>(filter.PropertyName);

                        if (prop == null)
                        {
                            throw new ArgumentException($"Filtered entity has no field with name {filter.PropertyNameSource}.");
                        }

                        return _filterValueParserFactory.GetValueParser(prop);
                    }),
                null);
        }

        private object[] ParseFilterValues<TEntity>(IFiltering filtering)
        {
            Filter[] queryFilters = filtering.GetQueryFilters();
            Dictionary<string, IValueParser> fieldParsers = GetPropertyFieldParsers<TEntity>(queryFilters, filtering.GetFilterSetUniqueName());

            return queryFilters.Select(filter =>
                {
                    try
                    {
                        return fieldParsers[filter.PropertyName].Parse(filter.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"Inappropriate filter value '{filter.Value}' for field with name {filter.PropertyNameSource}.", ex);
                    }
                })
                .ToArray();
        }
    }
}