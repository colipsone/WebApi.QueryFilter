using System;
using System.Linq;
using System.Linq.Expressions;
using IQueryableFilter.Infrastructure.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class FilterExpressionBuilder : IFilterExpressionBuilder
    {
        private readonly IFilterExpressionFactory _filterExpressionFactory;
        private readonly INamedFilterExpressionFactory _namedFilterExpressionFactory;

        public FilterExpressionBuilder(INamedFilterExpressionFactory namedFilterExpressionFactory,
            IFilterExpressionFactory filterExpressionFactory)
        {
            _namedFilterExpressionFactory = namedFilterExpressionFactory ??
                                            throw new ArgumentNullException(nameof(namedFilterExpressionFactory));
            _filterExpressionFactory = filterExpressionFactory ??
                                       throw new ArgumentNullException(nameof(filterExpressionFactory));
        }

        public Expression<Func<TEntity, bool>> GetNamedFiltersExpressionPredicate<TEntity>(IFiltering filtering)
            where TEntity : class, new()
        {
            Filter[] namedFilters = filtering.GetNamedFilters();

            if (namedFilters.Length == 0) return entity => true;

            return _namedFilterExpressionFactory.GetNamedFilterPredicates<TEntity>(namedFilters)
                .Aggregate((current, predicate) => current.Or(predicate));
        }

        public Expression<Func<TEntity, bool>> GetQueryFiltersExpressionPredicate<TEntity>(IFiltering filtering)
            where TEntity : class, new()
        {
            return _filterExpressionFactory.GetFilterExpression<TEntity>(filtering);
        }
    }
}