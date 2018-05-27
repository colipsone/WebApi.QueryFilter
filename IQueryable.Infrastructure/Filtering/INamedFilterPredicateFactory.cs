using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal interface INamedFilterPredicateFactory
    {
        Expression<Func<TEntity, bool>>[] GetNamedFilterPredicates<TEntity>(Filter[] namedFilters) where TEntity : class, new();
    }
}