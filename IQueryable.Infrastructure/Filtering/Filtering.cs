using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IQueryableFilter.Infrastructure.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public class Filtering : IFiltering
    {
        private readonly IFilterExpressionFactory _filterExpressionFactory;
        
        private readonly INamedFilterExpressionFactory _namedFilterExpressionFactory;
        private readonly IFilterItemsParser _filterItemsParser;

        private Filter[] _filters;

        private bool _isInitialized;

        public Filtering(IFilterExpressionFactory filterExpressionFactory,
            INamedFilterExpressionFactory namedFilterExpressionFactory, 
            IFilterItemsParser filterItemsParser)
        {
            _filterExpressionFactory = filterExpressionFactory ??
                                       throw new ArgumentNullException(nameof(filterExpressionFactory));
            _namedFilterExpressionFactory = namedFilterExpressionFactory ??
                                            throw new ArgumentNullException(nameof(namedFilterExpressionFactory));
            _filterItemsParser = filterItemsParser ??
                                 throw new ArgumentNullException(nameof(filterItemsParser));
        }

        public Expression<Func<TEntity, bool>> GetNamedFiltersPredicate<TEntity>() where TEntity : class, new()
        {
            return _namedFilterExpressionFactory.GetNamedFilterPredicates<TEntity>(GetNamedFilters())
                .Aggregate((current, predicate) => current.Or(predicate));
        }

        public Expression<Func<TEntity, bool>> GetQueryFiltersPredicate<TEntity>() where TEntity : class, new()
        {
            return _filterExpressionFactory.GetFilterExpression<TEntity>(GetQueryFilters());
        }

        public void Initialize(IDictionary<string, string[]> queryStringParameters)
        {
            if (queryStringParameters == null)
            {
                throw new ArgumentNullException(nameof(queryStringParameters));
            }

            if (_isInitialized)
            {
                throw new ApplicationException("Filter already initialized. This operation can be done only once per request!");
            }
            _filters = _filterItemsParser.ParseFilters(queryStringParameters);
            _isInitialized = true;
        }

        private Filter[] GetNamedFilters()
        {
            return _filters.Where(filter => filter.Operation.Type == FilterOperationType.Named).ToArray();
        }

        private Filter[] GetQueryFilters()
        {
            return _filters.Where(filter => filter.Operation.Type == FilterOperationType.Regular).ToArray();
        }
    }
}