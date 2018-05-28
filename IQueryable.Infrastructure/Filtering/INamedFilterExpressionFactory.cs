using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal interface INamedFilterExpressionFactory
    {
        Expression<Func<TEntity, bool>>[] GetNamedFilterPredicates<TEntity>(Filter[] namedFilters) where TEntity : class, new();
    }
}