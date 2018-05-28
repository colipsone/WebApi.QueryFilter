using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IQueryableFilter.Infrastructure.Common;
using IQueryableFilter.Infrastructure.Expressions;
using static System.Linq.Dynamic.DynamicExpression;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class FilterExpressionBuilder : IFilterExpressionBuilder
    {
        private readonly INamedFilterFactory _namedFilterFactory;
        private readonly IFilterExpressionFactory _filterExpressionFactory;

        public FilterExpressionBuilder(INamedFilterFactory namedFilterFactory, 
            IFilterExpressionFactory filterExpressionFactory)
        {
            _namedFilterFactory = namedFilterFactory ?? throw new ArgumentNullException(nameof(namedFilterFactory));
            _filterExpressionFactory = filterExpressionFactory ?? throw new ArgumentNullException(nameof(filterExpressionFactory));
        }

        public Expression<Func<TEntity, bool>> GetNamedFiltersExpressionPredicate<TEntity>(IFiltering filtering)
            where TEntity : class, new()
        {
            Filter[] namedFilters = filtering.GetNamedFilters();

            if (namedFilters.Length == 0) return entity => true;

            return _namedFilterFactory.GetNamedFilterPredicates<TEntity>(namedFilters)
                .Aggregate((current, predicate) => current.Or(predicate));
        }

        public Expression<Func<TEntity, bool>> GetQueryFiltersExpressionPredicate<TEntity>(IFiltering filtering)
            where TEntity : class, new()
        {
            return _filterExpressionFactory.GetFilterExpression<TEntity>(filtering);
        }
    }
}